using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSceneController : MonoBehaviour {

	public int dotsSelected = 0;

	// this variable is for when a mouse is being clicked/dragged
	public bool isScreenBeingTouched = false;

	// number of dots on board (has to be a square and even number otherwise will not work correctly)
	private static int numberOfDots = 36;
	private int numberOfRows;

	// create dot sprite that all cloned dots will use
	public WhiteDotObject dot;
	private List<WhiteDotObject> dotList;

	// previous dot
	public WhiteDotObject previousDot = null;

	// Use this for initialization
	void Start () {

		dotList = new List<WhiteDotObject>();

		// get the number of rows/columns
		numberOfRows = (int) Mathf.Sqrt (numberOfDots);

		// create dots
		spawnDots();
	}


	// Update is called once per frame
	void Update () {

		// once mouse is not being clicked, unselected any selected dots
		if (isScreenBeingTouched == false) {

			unselectAndRemoveDots ();
		}
	}

	void unselectAndRemoveDots() {

		// iterate through dot list
		for (int i = 0; i < numberOfDots; i++) {

			WhiteDotObject dotObject = dotList[i];

			// only execute if a dot is actually selected
			if (dotObject.isSelected) {

				// check if a square has been created first
				if (dotObject.squareCompleted) {

					dotObject.squareCompleted = false;

					// if square has been created then select all dots that have the same colour as the selected squares
					foreach (WhiteDotObject selectedDotObject in dotList) {

						if (selectedDotObject.GetComponent<SpriteRenderer>().color == previousDot.GetComponent<SpriteRenderer>().color) {

							selectedDotObject.isSelected = true;
						}
					}

					previousDot = null;

					// recursion to remove dots once they have been selected
					unselectAndRemoveDots();
				
				// make sure that at least 2 dots have been selected
				} else if (dotsSelected > 1) {

					// the whole point of startPos is to get the starting position within the array so that ONLY VERTICAL dots around the
					// selected dot are moved down. This is to save time so that other dots are not checked that do not need to be moved down
					int startPos = -numberOfRows/2;

					startPos = (int)dotObject.transform.position.x - startPos;

					startPos *= numberOfRows;

					// this for loop will check all dots above and below the selected dot
					for (int j = startPos; j < (startPos+numberOfRows); j++) {

						// any dots that are above the selected dot are moved down
						if (dotList[j].transform.position.y > dotObject.transform.position.y) {

							// first, make sure dot is in position to be moved down (for animation purposes)
							dotList[j].transform.position = new Vector2(dotList[j].transform.position.x, dotList[j].clampYPos);

							// set the physics to happen automatically
							dotList[j].rigidbody2D.isKinematic = false;

							// decrement the clamp Y posision so that the dot only falls one position downwards in the y axis
							dotList[j].clampYPos--;
						}
					}

					// destroy original selected dot
					Destroy(dotObject.gameObject);

					// remove dot from list
					dotList.RemoveAt(i);

					// create new dot at the top of the dot grid
					Vector2 spawnPosition = new Vector2 (dotObject.transform.position.x, numberOfRows/2);

					// insert new initialized dot in old dots place
					dotList.Insert(i, Instantiate(dot, spawnPosition, Quaternion.identity) as WhiteDotObject);

					// allow dot to drop
					dotList[i].rigidbody2D.isKinematic = false;

					// set the clamp position to be one below starting position (animation purposes)
					dotList[i].clampYPos = spawnPosition.y - 1;

					// set the minimum Y position that the dot can drop to
					dotList[i].minimumYPos = -numberOfRows/2;
				}

				dotObject.isSelected = false;
			}
		}

		previousDot = null;
		dotsSelected = 0;
	}

	private void spawnDots() {

		Vector2 spawnPosition;

		int count = 0;

		// create an x by x grid of dots
		for (int row = -numberOfRows/2; row < numberOfRows/2; row++) {

			for (int column = -numberOfRows/2; column < numberOfRows/2; column++) {

				spawnPosition = new Vector2 (row,column);

				// create a clone of the dot object
				dotList.Add(Instantiate(dot, spawnPosition, Quaternion.identity) as WhiteDotObject);

				// clamp position set from the start so that the y position is constantly remembered
				dotList[count].clampYPos = column;

				// set the lowest point that the dot can drop to.
				dotList[count++].minimumYPos = -numberOfRows/2;
			}
		}
	}
}
