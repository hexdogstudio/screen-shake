using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class ShakeRequestManagerCinemachine : CinemachineExtension
{
    [SerializeField] private Vector3 m_Axis = new Vector3(1, 1, 1);
    [SerializeField] private EaseCurve m_DampingCurve = new EaseCurve(Ease.Type.OutCubic);
    private readonly List<ShakeRequest> m_Requests = new List<ShakeRequest>();

    public Vector3 axis { get => m_Axis; set => m_Axis = value; }


    public void Request(float amplitude, float duration, Vector3 source = default)
    {
        m_Requests.Add(new ShakeRequest(amplitude, duration, source));
        if (m_Requests.Count - 1 == 0)
            StartCoroutine(ShakeExecutor());
        else
            m_Requests.Sort();
    }

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body)
            state.PositionCorrection += GetOffset();
    }

    private Vector3 GetOffset()
    {
        if (m_Requests.Count > 0)
        {
            var peak = m_Requests[0];
            Vector3 dir = (transform.position - peak.source).normalized;
            Vector3 offset = Random.insideUnitSphere + dir;
            float amp = m_DampingCurve.Calc(1 - peak.progress) * peak.amplitude;

            return new Vector3(
                offset.x * m_Axis.x * amp,
                offset.y * m_Axis.y * amp,
                offset.z * m_Axis.z * amp
            );
        }
        else
            return Vector3.zero;
    }

    private IEnumerator ShakeExecutor()
    {
        while (m_Requests.Count > 0)
        {
            bool cleanup = false;
            foreach (var request in m_Requests)
            {
                request.lifespan -= Time.deltaTime;
                if (request.lifespan <= 0)
                    cleanup = true;
            }

            if (cleanup)
                m_Requests.RemoveAll(req => req.lifespan <= 0);

            if (m_Requests.Count > 0)
                yield return null;
        }
    }

    public static implicit operator bool(ShakeRequestManagerCinemachine instance) => instance != null;

    private class ShakeRequest : System.IComparable<ShakeRequest>
    {
        private readonly float m_Amplitude;
        private readonly float m_Duration;
        private readonly Vector3 m_Source;
        private float m_Lifespan;

        public ShakeRequest(float amplitude, float duration, Vector3 source)
        {
            m_Amplitude = amplitude;
            m_Duration = duration;
            m_Source = source;
            m_Lifespan = duration;
        }

        public float amplitude => m_Amplitude;
        public float duration => m_Duration;
        public Vector3 source => m_Source;
        public float lifespan { get => m_Lifespan; set => m_Lifespan = value; }
        public float progress => Mathf.Clamp01(m_Lifespan / m_Duration);

        public int CompareTo(ShakeRequest other)
        {
            return other.m_Amplitude.CompareTo(m_Amplitude);
        }
    }
}
