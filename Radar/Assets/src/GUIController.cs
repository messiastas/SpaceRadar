using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIController : MonoBehaviour {

	public Text callsign0;
	public Text callsign1;
	public Text callsign2;
	public Text callsign3;

	public Text health0;
	public Text health1;
	public Text health2;

	public Text strength0;
	public Text strength1;
	public Text strength2;

	public Text intellect0;
	public Text intellect1;
	public Text intellect2;

	public Text social0;
	public Text social1;
	public Text social2;

	public Button canalAlpha;
	public Button canalDelta;
	public Button canalOmega;
	public Button canalTeam;

	public Button dangerUp;
	public Button dangerDown;
	public Button dangerLeft;
	public Button dangerRight;

	public Button checkUp;
	public Button checkDown;
	public Button checkLeft;
	public Button checkRight;

	protected int currentChannel = 3;
	protected AudioClip currentCanal;
	protected AudioClip currentCommand;
	protected AudioClip currentDirection;

	protected FightController gameController;

	void Awake () {
		onAwake();
	}

	protected virtual void onAwake()
	{
		gameController = gameObject.GetComponent<FightController>();

		callsign0.text = SharedVars.Inst.callsign0;
		callsign1.text = SharedVars.Inst.callsign1;
		callsign2.text = SharedVars.Inst.callsign2;
		callsign3.text = SharedVars.Inst.callsign3;

		onCanalClick(3);

	}

	public void onCanalClick(int index)
	{
		canalAlpha.GetComponentInChildren<Text>().color = Color.gray;
		canalDelta.GetComponentInChildren<Text>().color = Color.grey;
		canalOmega.GetComponentInChildren<Text>().color = Color.grey;
		canalTeam.GetComponentInChildren<Text>().color = Color.grey;
		currentChannel = index;
		switch (currentChannel)
		{
		case 0:
			canalAlpha.GetComponentInChildren<Text>().color = Color.green;
			currentCanal = gameController.snd_alphaunit;
			break;
		case 1:
			canalDelta.GetComponentInChildren<Text>().color = Color.green;
			currentCanal = gameController.snd_deltaunit;
			break;
		case 2:
			canalOmega.GetComponentInChildren<Text>().color = Color.green;
			currentCanal = gameController.snd_omegaunit;
			break;
		case 3:
			canalTeam.GetComponentInChildren<Text>().color = Color.green;
			currentCanal = gameController.snd_allteam;
			break;
		}
	}

	public void onDangerClick(string direction)
	{
		currentCommand = gameController.snd_enemyspotted;
		if(direction == "up") currentDirection = gameController.snd_toup;
		else if (direction=="down") currentDirection = gameController.snd_todown;
		else if (direction=="left") currentDirection = gameController.snd_toleft;
		else if(direction=="right") currentDirection = gameController.snd_toright;
		gameController.setSignal(currentChannel,"danger",direction,currentCanal,currentCommand,currentDirection);

	}

	public void onCheckClick(string direction)
	{
		currentCommand = gameController.snd_objectspotted;
		if(direction == "up") currentDirection = gameController.snd_toup;
		else if (direction=="down") currentDirection = gameController.snd_todown;
		else if (direction=="left") currentDirection = gameController.snd_toleft;
		else if(direction=="right") currentDirection = gameController.snd_toright;
		gameController.setSignal(currentChannel,"check",direction,currentCanal,currentCommand,currentDirection);

	}

	void OnGUI () {
		PointBase unit = gameController.GetUnit(0);
		health0.text = "Health:"+unit.health;
		strength0.text = "Strength:"+unit.strength;
		intellect0.text = "Intellect:"+unit.intellect;
		social0.text = "Social:"+unit.social + " "+unit.readyDirection;

		unit = gameController.GetUnit(1);
		health1.text = "Health:"+unit.health;
		strength1.text = "Strength:"+unit.strength;
		intellect1.text = "Intellect:"+unit.intellect;
		social1.text = "Social:"+unit.social + " "+unit.readyDirection;

		unit = gameController.GetUnit(2);
		health2.text = "Health:"+unit.health;
		strength2.text = "Strength:"+unit.strength;
		intellect2.text = "Intellect:"+unit.intellect;
		social2.text = "Social:"+unit.social + " "+unit.readyDirection;
	}
}
