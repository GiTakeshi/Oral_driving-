using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
 
public class CarController : MonoBehaviour
{

    public string microphoneDevice = null; // Имя микрофона (null для устройства по умолчанию)
    [SerializeField]public int sampleWindow = 64; // Размер окна для анализа звука
    [SerializeField]public float sensitivity = 100.0f; // Чувствительность к громкости
    [SerializeField]public float volumeThreshold = 0.01f; // Порог громкости для реакции
    private AudioClip microphoneClip;
    private bool isMicrophoneActive = false;
    //public float volumeVariable = 0.0f;

    private Rigidbody _rb;
    
    // [SerializeField] private Transform _masscentre;

    [SerializeField] private float _slipangle;

    [SerializeField] private AnimationCurve _steercurve;
    [SerializeField] private float _speed;
    [SerializeField] private Transform _transformFL;
    [SerializeField] private Transform _transformFR;
    [SerializeField] private Transform _transformBL;
    [SerializeField] private Transform _transformBR;
 
    [SerializeField] private WheelCollider _colliderFL;
    [SerializeField] private WheelCollider _colliderFR;
    [SerializeField] private WheelCollider _colliderBL;
    [SerializeField] private WheelCollider _colliderBR;
 
    [SerializeField] private float _force;
    private float _vol_for_cal;
    public bool isCalculating = true;
    public bool isRunning = false;
    private float timer = 0f;
    private List<float> values = new List<float>();

    private float average = 0;




    [SerializeField] private float _maxAngle;
    // Start is called before the first frame update
    private void Start(){
        _rb = GetComponent<Rigidbody>();
        //_rb.masscentre = _masscentre.position;
        if (Microphone.devices.Length > 0)
        {
            microphoneClip = Microphone.Start(microphoneDevice, true, 1, AudioSettings.outputSampleRate);
            isMicrophoneActive = true;
        }
    }
    private void FixedUpdate(){
        CalculateAverageOverTime();
        

    }
 
    private void Move()
    {
        _speed = _rb.linearVelocity.magnitude;
        
        _colliderFL.motorTorque = /*Input.GetAxis("Vertical") * */_force * _force;
        _colliderFR.motorTorque = /*Input.GetAxis("Vertical") * */_force * _force;

 
        if (Input.GetKey(KeyCode.Space))
        {
            _colliderFL.brakeTorque = 50000f;
            _colliderFR.brakeTorque = 50000f;
            _colliderBL.brakeTorque = 50000f;
            _colliderBR.brakeTorque = 50000f;
        }
        else
        {
            _colliderFL.brakeTorque = 0f;
            _colliderFR.brakeTorque = 0f;
            _colliderBL.brakeTorque = 0f;
            _colliderBR.brakeTorque = 0f;
        }
 
        
        _colliderFL.steerAngle = _maxAngle * Input.GetAxis("Horizontal");
        _colliderFR.steerAngle = _maxAngle * Input.GetAxis("Horizontal");


        RotateWheel(_colliderFL, _transformFL);
        RotateWheel(_colliderFR, _transformFR);
        RotateWheel(_colliderBL, _transformBL);
        RotateWheel(_colliderBR, _transformBR);

        VoiceControl();


    }
 
    private void RotateWheel(WheelCollider collider, Transform transform)
    {
        Vector3 position;
        Quaternion rotation;
 
        collider.GetWorldPose(out position, out rotation);
 
        transform.rotation = rotation;
        transform.position = position;
    }
    private void VoiceControl()
    {
        if (isMicrophoneActive)
        {
            // Получаем громкость звука с микрофона
            float _volume = GetVolumeFromMicrophone();

            // Изменяем переменную в зависимости от громкости
            //float calibration_var = CalculateAverageOverTime();
            //_force = _volume * sensitivity * _rb.mass;
            if(_force < 0)
            {
                _force = 0;
                _colliderFL.brakeTorque = 50000f;
                _colliderFR.brakeTorque = 50000f;
                _colliderBL.brakeTorque = 50000f;
                _colliderBR.brakeTorque = 50000f;

            }
    

            // Выводим значение переменной в консоль*/
            //Debug.Log("Volume Variable: " + _force);
            
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

    void CalculateAverageOverTime()
    {
        //float average = 0;

        if (isCalculating == true)
        {

            timer += Time.deltaTime;
            if (timer < 10f)
            {
                _vol_for_cal = GetVolumeFromMicrophone();
                values.Add(_vol_for_cal);
                //Debug.Log(values);

            }
            else
            {
                isRunning = true;
                isCalculating = false;
                float sum = 0f;
                foreach (float value in values){
                    sum += value;
                }
                average = sum / values.Count * _rb.mass * sensitivity;
                Debug.Log("average:" + average);
                
            }
        }
        _force = GetVolumeFromMicrophone() * sensitivity * _rb.mass - average;
        //Debug.Log(_force);
        if(isRunning == true){
            Move();
        }

        
    }

}
 