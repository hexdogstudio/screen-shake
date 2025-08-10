using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShakeSource : MonoBehaviour
{
    [SerializeField] private Gradient m_AmplitudeColor;
    [SerializeField] private Camera m_TargetCamera;
    [SerializeField] private ShakeEffect m_Indicator;
    [SerializeField] private TextMeshProUGUI m_ProfileIndicator;
    [SerializeField] private ShakeRequestManagerCinemachine m_ShakeManager;
    [SerializeField] private Image m_ProfileIndicatorFrame;

    private float m_MaxAmplitude;
    private int m_TargetProfile;
    private readonly ShakeProfile[] m_Profiles = new[]{
        new ShakeProfile(0.1f, 4.0f),
        new ShakeProfile(0.25f, 2.5f),
        new ShakeProfile(0.5f, 1.0f),
        new ShakeProfile(0.75f, 0.5f),
        new ShakeProfile(1.5f, 0.5f)
    };

    void Awake()
    {
        m_MaxAmplitude = float.MinValue;

        foreach (var profile in m_Profiles)
            if (profile.amplitude > m_MaxAmplitude)
                m_MaxAmplitude = profile.amplitude;

        UpdateProfileDisplay();
    }
    void Update()
    {
        SwitchProfiles();

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPoint = m_TargetCamera.ScreenToWorldPoint(Input.mousePosition);
            var profile = m_Profiles[m_TargetProfile];
            ShakeAt(profile.amplitude, profile.duration, worldPoint);
        }
    }

    private void ShakeAt(float amplitude, float duration, Vector3 worldPoint)
    {
        var indicator = Instantiate(m_Indicator);
        indicator.PlayAt(amplitude * 10, duration, worldPoint, m_AmplitudeColor.Evaluate(amplitude / m_MaxAmplitude));
        m_ShakeManager.Request(amplitude, duration, worldPoint);
    }
    private void SwitchProfiles()
    {
        int old = m_TargetProfile;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            m_TargetProfile = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            m_TargetProfile = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            m_TargetProfile = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            m_TargetProfile = 3;
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            m_TargetProfile = 4;

        if (old != m_TargetProfile)
            UpdateProfileDisplay();
    }
    private void UpdateProfileDisplay()
    {
        Color color = m_AmplitudeColor.Evaluate(m_Profiles[m_TargetProfile].amplitude / m_MaxAmplitude);
        m_ProfileIndicator.text = (m_TargetProfile + 1).ToString();
        m_ProfileIndicator.color = color;
        m_ProfileIndicatorFrame.color = color;
    }

    public static implicit operator bool(ShakeSource instance) => instance != null;

    private struct ShakeProfile
    {
        public float amplitude;
        public float duration;

        public ShakeProfile(float amplitude, float duration)
        {
            this.amplitude = amplitude;
            this.duration = duration;
        }
    }
}
