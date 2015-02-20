using UnityEngine;
using System.Collections;

public class WhiteDotObject : MonoBehaviour {

	static private float connectedLineWidth = 0.15f;
	public WhiteDotObject previousDot = null;

	// 'when selected' variables
	public bool isSelected = false;
	public bool squareCompleted = false;

	// clamping the dots into position (dots dropping animation)
	public float clampYPos = 0;
	public float minimumYPos = 0;

	LineRenderer lineRenderer;

	// gameSceneController access
	GameSceneController gameLoopScript;
	GameObject gameLoop;

	// Use this for initialization
	void Start () {

		// set the colour of the white dot once the dot is initialized
		setDotColour();

		lineRenderer = (LineRenderer) gameObject.AddComponent("LineRenderer");
		lineRenderer.material = new Material (Shader.Find("Self-Illumin/Diffuse"));
		lineRenderer.material.color = GetComponent<SpriteRenderer> ().color;
		lineRenderer.SetWidth (connectedLineWidth, connectedLineWidth);

		gameLoop = GameObject.Find("MainGameLoop");
		gameLoopScript = gameLoop.GetComponent<GameSceneController> ();   
    }

	private void OnDestroy() {

		Destroy(lineRenderer.material);
	}

	// Update is called once per frame
	void Update () {

		// once the dot stops moving after bouncing, clamp the dot into position
		// second conditional is for the dots at the bottom, so that they dont fall through the bottom of the screen
		if ((transform.position.y < clampYPos && rigidbody2D.velocity == Vector2.zero && !rigidbody2D.isKinematic) ||
		    (transform.position.y < minimumYPos)) {

			// disable automatic physics
			rigidbody2D.isKinematic = true;

			// clamp dots to nearest int position
			transform.position = new Vector2 (Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));

			// then move dot into clamped position
			transform.position = new Vector2 (Mathf.Round(transform.position.x), clampYPos);
		}

		if (gameLoopScript.previousDot == this && !gameLoopScript.previousDot.squareCompleted) {

			drawConnectingLine();
        }
	}

	void OnMouseEnter() {

		// if dot has been touched and is ready to connect to another dot
		if (gameLoopScript.isScreenBeingTouched) {

			if (!gameLoopScript.previousDot) {
			
				gameLoopScript.previousDot = this;
				gameLoopScript.dotsSelected++;
				isSelected = true;

			} else if (!gameLoopScript.previousDot.squareCompleted) {
			
				if (!isSelected) {

					// the purpose of x and y is to get the difference in between two connected dots in terms of coordinates
					double x = Mathf.Abs(gameLoopScript.previousDot.transform.position.x - transform.position.x);
					double y = Mathf.Abs(gameLoopScript.previousDot.transform.position.y - transform.position.y);
					
					// if total difference is either 1 or -1 then it means the dots are horizontally or vertically connected
					// if more/less then the dots are diagonal which is incorrect. if x+y = 0 then it means that its the same dot

					if ((x + y) == 1 || (x + y) == -1) {
						
						// then check if both dots are the same colour
						if (gameLoopScript.previousDot.GetComponent<SpriteRenderer>().color == GetComponent<SpriteRenderer>().color) {
							
							// if both conditions are satisfied then select dot
							isSelected = true;
							gameLoopScript.previousDot.drawConnectedLine(transform.position);
							this.previousDot = gameLoopScript.previousDot;
							gameLoopScript.previousDot = this;
							gameLoopScript.dotsSelected++;
							lineRenderer.SetVertexCount(2); 
						}
					}

				} else if (gameLoopScript.previousDot.previousDot == this) {

					gameLoopScript.previousDot.isSelected = false;
					gameLoopScript.previousDot.removeConnectedLine();
					gameLoopScript.previousDot.previousDot = null;
					gameLoopScript.previousDot = this;
					gameLoopScript.dotsSelected--;

				} else if (gameLoopScript.previousDot != this && gameLoopScript.previousDot.previousDot != this) {

					double x = Mathf.Abs(gameLoopScript.previousDot.transform.position.x - transform.position.x);
					double y = Mathf.Abs(gameLoopScript.previousDot.transform.position.y - transform.position.y);

					if ((x + y) == 1 || (x + y) == -1) {

						squareCompleted = true;
						gameLoopScript.previousDot.drawConnectedLine(this.transform.position);
						gameLoopScript.previousDot = this;
						isSelected = true;
					}
				}
			}
		}
	}

	void OnMouseDown() {

		lineRenderer.SetVertexCount(2); 
		gameLoopScript.isScreenBeingTouched = true;
		OnMouseEnter();
	}

	public void removeConnectedLine() {
	
		lineRenderer.SetVertexCount(0);
	}

	public void drawConnectedLine(Vector2 endPos) {

		lineRenderer.SetPosition (0, transform.position);
		lineRenderer.SetPosition (1, endPos);
    }

	private void drawConnectingLine() {

		lineRenderer.SetPosition (0, transform.position);
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		lineRenderer.SetPosition (1, new Vector2(mousePos.x, mousePos.y));
	}

	void OnMouseUp() {

		lineRenderer.SetVertexCount(0); 
		gameLoopScript.isScreenBeingTouched = false;
	}

	private void setDotColour() {

		// sets a random dot colour once the dot has been initialized

		switch(Random.Range(0, 5)) {

		case 0:
			// Purple
			GetComponent<SpriteRenderer>().color = new Color(0.729f, 0.333f, 0.827f, 1.000f);
			break;

		case 1:
			// Blue
			GetComponent<SpriteRenderer>().color = new Color(0.118f, 0.565f, 1.000f, 1.000f);
			break;
		
		case 2:
			// Red
			GetComponent<SpriteRenderer>().color = new Color(1.000f, 0.388f, 0.278f, 1.000f);
			break;

		case 3:
			// Green
			GetComponent<SpriteRenderer>().color = new Color(0.565f, 0.933f, 0.565f, 1.000f);
			break;

		default:
			// Yellow
			GetComponent<SpriteRenderer>().color = new Color(1.000f, 0.843f, 0.000f, 1.000f);
			break;
		}
	}
}
