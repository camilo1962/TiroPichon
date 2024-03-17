using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    private Vector3 _startPosLeft, _startPosRight;

    [SerializeField]
    private float _moveTime;
    private void OnEnable()
    {

        GameManager.Instance.ColorChanged1 += ColorChanged1;
    }

    private void OnDisable()
    {

        GameManager.Instance.ColorChanged1 -= ColorChanged1;
    }
    private void Start()
    {
        transform.position = Random.Range(0, 1) > 0.5f ? _startPosLeft : _startPosRight;
    }

    public void MoveToPos(Vector3 targetPos)
    {
        StartCoroutine(IMoveToPos(targetPos));
    }

    private IEnumerator IMoveToPos(Vector3 targetPos)
    {
        float timeElapsed = 0f;
        while(timeElapsed < _moveTime)
        {
            timeElapsed += Time.fixedDeltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, _moveTime);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        transform.position = targetPos;
    }
    public void ColorChanged(Color col)
    {
        GetComponent<SpriteRenderer>().color = col;
        var mm = GetComponent<ParticleSystem>().main;
        mm.startColor = col;
    }
    #region COLOR_CHANGE
    [SerializeField] private List<Color> _colors;

    [HideInInspector] public Color CurrentColor => _colors[CurrentColorId];

    [HideInInspector] public UnityAction<Color> ColorChanged1;

    private int _currentColorId;
    public int CurrentColorId
    {
        get
        {
            return _currentColorId;
        }

        set
        {
            _currentColorId = value % _colors.Count;
            ColorChanged1?.Invoke(CurrentColor);
        }
    }

    #endregion
}
