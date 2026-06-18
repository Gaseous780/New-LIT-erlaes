using System.Collections;
using UnityEngine;

public class FryerBehaviour : MonoBehaviour
{
    [SerializeField] private float timeNeeded;

    [SerializeField] private float timeToFail;
    private float timer;
    private int direction;

    private float actualTremble;

    [SerializeField] private float trembleSpeed;
    private float defaultTrembleSpeed;
    [SerializeField] private float increasingTrembleSpeed;
    private float defaultIncreasingTrembleSpeed;
    private Vector3 positionOriginal;

    private bool isOnFry;

    private bool canFry; //Si llego a la posición para freir (si bajo lo suficiente)
    private float positionOn; //La posición en la que se muve la freidora hacia arriba o abajo
    private int directionToGo; //Para cuando lo sube o baja

    [SerializeField] private ProgressiveBar progressiveBar;

    [SerializeField] private ParticleSystem[] dropOil; 

    private void Awake()
    {
        positionOriginal = transform.position;

        defaultTrembleSpeed = trembleSpeed;
        defaultIncreasingTrembleSpeed = increasingTrembleSpeed;
    }

    private void OnEnable()
    {
        actualTremble = 0;
        isOnFry = false;
        canFry = false;
        direction = 1;
        positionOn = 0;
        directionToGo = -1;

        trembleSpeed = defaultTrembleSpeed;
        increasingTrembleSpeed = defaultIncreasingTrembleSpeed;
    }

    private void Update()
    {
        if (isOnFry == true && canFry == false)
        {
            timer += Time.deltaTime;

            actualTremble += (trembleSpeed * direction) * Time.deltaTime;

            if (timer <= timeNeeded) { trembleSpeed += Time.deltaTime * increasingTrembleSpeed; }

            transform.position = new Vector3(Mathf.Lerp(positionOriginal.x + 0.5f, positionOriginal.x - 0.5f, actualTremble), positionOriginal.y, transform.position.z);

            if (actualTremble >= 1 || actualTremble <= 0)
            {
                actualTremble += (0.3f * -direction);
                direction *= -1;
            }

            if (dropOil[0].isPlaying == false)
            {
                foreach (ParticleSystem drop in dropOil)
                {
                    ParticleSystem.MainModule dropMain = drop.main;
                    dropMain.duration *= 9f;
                    drop.Play();
                }
            }
        }

        if (canFry == true)
        {
            ChangePositionFryer(directionToGo);
        }
    }

    public void HandDown()
    {
        if (isOnFry == false)
        {
            positionOn = 0;
            canFry = true;
            progressiveBar._canActivate = true;

            foreach (ParticleSystem drop in dropOil)
            {
                ParticleSystem.MainModule dropMain = drop.main;
                if (drop.isPlaying == false) { dropMain.duration = 0.3f; }
                drop.Play();
            }
        }
    }

    public float HandUp()
    {
        if (isOnFry == true && timer >= 1.9f)
        {
            progressiveBar.ResetProgress();
            progressiveBar._canActivate = false;

            isOnFry = false;
            canFry = true;
            actualTremble = 0;
            positionOn = 0;

            float timeOnFry = timer;
            timer = 0;
            trembleSpeed = defaultTrembleSpeed;
            increasingTrembleSpeed = defaultIncreasingTrembleSpeed;

            foreach (ParticleSystem drop in dropOil)
            {
                drop.Stop();
            }

            return timeOnFry;
        }

        return default;
    }

    private void ChangePositionFryer(int direction) //Si es 1 va a ir hacia arriba, si es -1 irá hacia abajo
    {
        //Debug.Log(directions);
        transform.position = new Vector3(positionOriginal.x, positionOriginal.y, Mathf.Lerp(transform.position.z, transform.position.z + direction, positionOn));
        positionOn += Time.deltaTime;

        switch (direction)
        {
            case 1:
                if (transform.position.z >= positionOriginal.z + direction)
                {
                    directionToGo *= -1;
                    canFry = false;
                }
                break;

            case -1:
                if (transform.position.z <= positionOriginal.z + direction)
                {
                    if (isOnFry == false)
                    {
                        isOnFry = true;
                    }

                    directionToGo *= -1;
                    canFry = false;
                }
                break;
        }
    }
}
