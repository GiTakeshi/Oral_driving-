using UnityEngine;

public class MicInput : MonoBehaviour
{
    public string microphoneDevice = null; // Имя микрофона (null для устройства по умолчанию)
    public int sampleWindow = 64; // Размер окна для анализа звука
    public float sensitivity = 100.0f; // Чувствительность к громкости
    public float volumeThreshold = 0.01f; // Порог громкости для реакции

    private AudioClip microphoneClip;
    private bool isMicrophoneActive = false;

    // Переменная, которая будет изменяться в зависимости от громкости
    public float volumeVariable = 0.0f;

    void Start()
    {
        // Инициализация микрофона
        if (Microphone.devices.Length > 0)
        {
            microphoneClip = Microphone.Start(microphoneDevice, true, 1, AudioSettings.outputSampleRate);
            isMicrophoneActive = true;
        }
        else
        {
            Debug.LogError("Микрофон не найден!");
        }
    }

    void Update()
    {
        if (isMicrophoneActive)
        {
            // Получаем громкость звука с микрофона
            float volume = GetVolumeFromMicrophone();

            // Изменяем переменную в зависимости от громкости
            volumeVariable = volume * sensitivity;

            // Выводим значение переменной в консоль
            Debug.Log("Volume Variable: " + volumeVariable);
        }
    }

    float GetVolumeFromMicrophone()
    {
        float[] waveData = new float[sampleWindow];
        int micPosition = Microphone.GetPosition(microphoneDevice) - (sampleWindow + 1);

        if (micPosition < 0)
        {
            return 0;
        }

        microphoneClip.GetData(waveData, micPosition);

        // Вычисляем среднюю громкость
        float levelMax = 0;
        for (int i = 0; i < sampleWindow; i++)
        {
            float wavePeak = Mathf.Abs(waveData[i]);
            if (wavePeak > levelMax)
            {
                levelMax = wavePeak;
            }
        }

        return levelMax;
    }

    void OnDestroy()
    {
        // Останавливаем микрофон при уничтожении объекта
        if (isMicrophoneActive)
        {
            Microphone.End(microphoneDevice);
        }
    }
}