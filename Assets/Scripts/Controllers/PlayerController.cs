using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;




public class PlayerController : MonoBehaviour
{
	[SerializeField] GameEvent shootEvent;


	private PointerEventData pointerData;

    private bool isDragging = false;
	private Vector2 StartPosition;
	private Vector2 CurrentPosition;
	private Vector2 TotalDeltaPosition;
	private Vector2 LastPosition;
	private Vector2 DeltaPosition;

	private void Start()
	{
		isShootAllowed = true;
	}

	public void TouchDown(BaseEventData data)
	{
		pointerData = data as PointerEventData;

		StartPosition = pointerData.position;
		LastPosition = StartPosition;

		FirstTouch();
		ShowPrediction();
	}

	private void FirstTouch()
	{		
		Vector2 newPos = Camera.main.ScreenToWorldPoint(StartPosition);

		Vector2 direction = new Vector2(
				newPos.x - DirectionContainer.transform.position.x,
				newPos.y - DirectionContainer.transform.position.y
				);

		DirectionContainer.transform.up = direction;		
	}

	public void TouchDrag(BaseEventData data)
    {
        pointerData = data as PointerEventData;

        isDragging = true;

        CurrentPosition = pointerData.position;

        TotalDeltaPosition = CurrentPosition - StartPosition;
        DeltaPosition = CurrentPosition - LastPosition;

        LastPosition = pointerData.position;

		
		RotatePlayer();

	}    
	
   

	public void TouchUp()
    {
		HidePrediction();
		Shoot();
	}



	// ******* Rotation ******

	[SerializeField] private float speedRotation;
	void RotatePlayer()
	{		
		DirectionContainer.transform.Rotate(0, 0, -DeltaPosition.x * Time.deltaTime * speedRotation);
	}


	// ******* Calculating Prediction ******

	void ShowPrediction()
	{
		raycsatCoroutine = StartCoroutine(DoRaycastRoutine());
	}

	private Coroutine raycsatCoroutine;

	private Vector3 currentDirection;
	private Vector3 newDir;
	private Vector3 ray2Point;
	private Vector3 ray1Point;
	private Vector3 predictedPos;

	[SerializeField] GameObject DirectionContainer;
	[SerializeField] private LayerMask WallLayerMask;
	[SerializeField] private LayerMask BallLayerMask;
	[SerializeField] private LayerMask PredictionLayerMask;

	[SerializeField] private BallController currentBallController;

	private readonly string wallTag = "Wall";
	private readonly string ballTag = "Ball";
	private string lastName;
	private string currName;


	IEnumerator DoRaycastRoutine ()
    {
		Prediction.gameObject.SetActive(true);
		while (true)
        {

			//yield return new WaitUntil(() => isShootAllowed == true);


			Vector3 origin = transform.position;
            Vector3 direction = DirectionContainer.transform.up;


			//First Ray
			Debug.DrawRay(origin, direction * 8, Color.green);			
			RaycastHit2D firstRay = Physics2D.Raycast(origin, direction, 10, WallLayerMask);


			ray1Point = firstRay.point;

			float NormalsAngle = Vector3.Angle(firstRay.normal, new Vector3(1, 0, 0));
            if (firstRay.normal.x < 0) NormalsAngle *= -1;
               
            float angel =Vector3.Angle(direction, firstRay.normal);

            float a = 90 - ( 180 - angel );
            a = (NormalsAngle - 90) + a;

            Vector3 wallDir = new Vector3(Mathf.Cos((NormalsAngle - 90) * Mathf.Deg2Rad), Mathf.Sin((NormalsAngle - 90) * Mathf.Deg2Rad), 0);
            float wallDirAngle = Vector3.Angle(direction, wallDir);

            if (wallDirAngle > 90) a = 180 - a;
                

            if(a >= 360)
            {
                a = a - 180;
            }
            else if (a > 180 && a < 270)
            {
                a = a - 180;
            }

            newDir = new Vector3(Mathf.Cos(a * Mathf.Deg2Rad), Mathf.Sin(a * Mathf.Deg2Rad), 0);
			currentDirection = newDir;



			if ((firstRay.collider!=null) && (firstRay.collider.tag.Equals(wallTag)))
			{

				lineRenderer2.enabled = true;
				isRebound = true;

				//Second Ray
				Debug.DrawRay(firstRay.point, currentDirection * 8, Color.green);
				RaycastHit2D secondRay = Physics2D.Raycast(firstRay.point, currentDirection , 10,BallLayerMask);

				ray2Point = secondRay.point;

				if (secondRay.collider != null)
				{
					OnOffLines(true, true);
					isShootAllowed = true;

					currentDirection = (Vector3)secondRay.point - secondRay.collider.transform.position;
					DoFinalRaycast(secondRay.point, currentDirection.normalized);

				}
				else 
				{
					OnOffLines(false, false);
					isShootAllowed = false;
				}
			
				

				
			}
			else if ((firstRay.collider != null) && (firstRay.collider.tag.Equals(ballTag)))
			{
				OnOffLines(true, false);
				isRebound = false;

				currentDirection = (Vector3)firstRay.point - firstRay.collider.transform.position;
				DoFinalRaycast(firstRay.point, currentDirection.normalized);
				
			}
			


			DrawLine(origin, firstRay.point, ray2Point, newDir);


			yield return new WaitForFixedUpdate();
        }
                      
    }

	[SerializeField] private PredictionController Prediction;
	void DoFinalRaycast(Vector3 origin, Vector3 dir)
	{
		Debug.DrawRay(origin, dir, Color.blue);
		RaycastHit2D secondRay = Physics2D.Raycast(origin, dir, 0.2f, PredictionLayerMask);
		
		if (secondRay.collider != null)
		{
			
			currName = secondRay.collider.name;

			if (currName != lastName)
			{			
				lastName = currName;

				currentBallController = secondRay.collider.gameObject.GetComponent<BallController>();
				Prediction.transform.position = currentBallController.transform.position;
				Prediction.ActivatePrediction(shootingBall.color);
				predictedPos = currentBallController.transform.position;			
			}
		}
	}

	

	void HidePrediction()
	{
		if (raycsatCoroutine != null) StopCoroutine(raycsatCoroutine);
		Prediction.gameObject.SetActive(false);
		OnOffLines(false, false);
	}



	// ******* Draw Line ******

	[SerializeField] private LineRenderer lineRenderer1;
	[SerializeField] private LineRenderer lineRenderer2;
	[SerializeField] private float TotalLength;

	private float L2;
	private float L1;


	void DrawLine(Vector3 startPos, Vector3 hitPos, Vector3 hitPos2, Vector3 endDir)
	{

		L1 = Vector3.Distance(hitPos, startPos);

		if (L1 < TotalLength)
		{
			lineRenderer1.SetPosition(0, startPos);
			lineRenderer1.SetPosition(1, hitPos);

			L2 = TotalLength - L1;

			lineRenderer2.SetPosition(0, hitPos - new Vector3(0, 0.06f, 0));
			lineRenderer2.SetPosition(1, hitPos2);
		}
		else
		{
			lineRenderer1.SetPosition(0, startPos);
			lineRenderer1.SetPosition(1, startPos + (hitPos - startPos).normalized * TotalLength);

			lineRenderer2.gameObject.SetActive(false);
		}
	}

	void OnOffLines(bool isFirstlineOn, bool isSecondlineOn)
	{
		lineRenderer1.enabled = isFirstlineOn;
		lineRenderer2.enabled = isSecondlineOn;		
	}



	// ******* Shoot ******


	[SerializeField] private AmoController shootingBall;
	
	private bool isShootAllowed;
	private bool isRebound;
	private int queue = 0;

	public void SwichShootingBalls()
	{
	}

	void Shoot()
	{	
		if (isShootAllowed)
		{
			shootEvent.Raise();
			StartCoroutine(AddFirstForce());
		}
	}



	void ShootingDone()
	{
		shootingBall.transform.position = transform.position;
		currentBallController.ActivateBall(shootingBall.color,shootingBall.number);
		shootingBall.InitAmo();
		isShootAllowed = true;
	}

	IEnumerator AddFirstForce()
	{
		isShootAllowed = false;
		float startTime = Time.time;
		float duration = 0.35f;
		float t = 0;

		Vector3 startPos = transform.position;
		Vector3 endPos = ray1Point;
		while (t < 0.9f)
		{
			t = (Time.time - startTime) / duration;
			shootingBall.transform.position = Vector3.Lerp(startPos, endPos, t);
			yield return new WaitForEndOfFrame();
		}


		if (isRebound) StartCoroutine(AddSecondForce());
		else ShootingDone();
	}

	IEnumerator AddSecondForce()
	{
		float startTime = Time.time;
		float duration = 0.25f;
		float t = 0;

		Vector3 startPos = ray1Point;
		Vector3 endPos = predictedPos;

		while (t < 1f)
		{
			t = (Time.time - startTime) / duration;
			shootingBall.transform.position = Vector3.Lerp(startPos, endPos, t);
			yield return new WaitForEndOfFrame();
		}

		shootingBall.transform.position = predictedPos;

		ShootingDone();
	}


	



}//end
