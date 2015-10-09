using UnityEngine;
using System.Collections;

public class FightController : MonoBehaviour {

	public GameObject screen0;
	public GameObject screen1;
	public GameObject screen2;
	public GameObject screen3;
	public Camera mainCamera;

	public GameObject screenAlert0;
	public GameObject screenAlert1;
	public GameObject screenAlert2;

	public GameObject capitanPoint;
	public GameObject soldierPoint;
	public GameObject medicPoint;
	public GameObject enemyPoint;
	public GameObject objectPoint;
	public GameObject missionTargetPoint;

	public AudioClip snd_alphaunit;
	public AudioClip snd_deltaunit;
	public AudioClip snd_omegaunit;
	public AudioClip snd_allteam;
	public AudioClip snd_enemyspotted;
	public AudioClip snd_objectspotted;
	public AudioClip snd_toleft;
	public AudioClip snd_toright;
	public AudioClip snd_toup;
	public AudioClip snd_todown;
	public AudioClip snd_roger;
	public AudioClip snd_screw;
	public AudioClip snd_enemycontact;
	public AudioClip snd_clear;
	public AudioClip snd_injured;
	public AudioClip snd_missioncomplete;

	protected GUIController guiControl;
	protected AudioSource canalDispetcher;
	protected AudioSource canalAlpha;
	protected AudioSource canalDelta;
	protected AudioSource canalOmega;

	public int enemyNumber = 10;
	public int objectsNumber = 3;
	public float screen0Freq = 1;
	public float screen1Freq = 1;
	public float screen2Freq = 1;
	public float screen3Freq = 1;


	protected float ortSize = 12;
	protected float freeWidth;
	protected float freeHeight;
	protected float everyScreenWidth=12f;
	protected float everyScreenHeight=11.9f;
	protected float personalScreenScale = 3f;
	protected float currentTime;
	protected float lastTimeAlertsClean = 0f;
	protected bool isExitMission = false;


	protected PointBase[] mans;
	protected GameObject[] allteamPoints;
	protected GameObject[] capitanPoints;
	protected GameObject[] soldierPoints;
	protected GameObject[] medicPoints;

	protected int capitanIndex;
	protected int soldierIndex;
	protected int medicIndex;
	protected int missionIndex;
	// Use this for initialization
	void Start()
	{
		onStart ();
	}

	protected virtual void onStart()
	{
		guiControl = gameObject.GetComponent<GUIController>();
		ortSize = mainCamera.orthographicSize;
		freeWidth = ortSize*2*16/9 -(ortSize*2*16/9)/3;
		freeHeight = ortSize*2;
		screen0.transform.position = new Vector3(0.25f,ortSize,screen0.transform.position.z);
		screen1.transform.position = new Vector3(everyScreenWidth+0.5f,ortSize,screen1.transform.position.z);
		screen2.transform.position = new Vector3(0.25f,0,screen2.transform.position.z);
		screen3.transform.position = new Vector3(everyScreenWidth+0.5f,0,screen3.transform.position.z);
		Debug.Log(everyScreenWidth);
		createOperationMap();
		currentTime = 0f;
		screenAlert0.SetActive(false);
		screenAlert1.SetActive(false);
		screenAlert2.SetActive(false);


		canalDispetcher = gameObject.AddComponent<AudioSource>();
		canalDispetcher.playOnAwake = false;
		canalDispetcher.enabled = true;
		canalDispetcher.volume = 0.5f;

		canalAlpha = gameObject.AddComponent<AudioSource>();
		canalAlpha.playOnAwake = false;
		canalAlpha.enabled = true;
		canalAlpha.volume = 0.5f;

		canalDelta = gameObject.AddComponent<AudioSource>();
		canalDelta.playOnAwake = false;
		canalDelta.enabled = true;
		canalDelta.volume = 0.5f;

		canalOmega = gameObject.AddComponent<AudioSource>();
		canalOmega.playOnAwake = false;
		canalOmega.enabled = true;
		canalOmega.volume = 0.5f;
	}

	protected void createOperationMap()
	{
		mans = new PointBase[enemyNumber+3+objectsNumber+1];
		mans[0] = new PointBase("capitan","team",15,15,this,0);
		mans[0].strength = Random.Range(1,SharedVars.Inst.charMax);
		mans[0].intellect = Random.Range(1,SharedVars.Inst.charMax);
		mans[0].social = Random.Range(1,SharedVars.Inst.charMax);
		capitanIndex = 0;
		mans[1] = new PointBase("soldier","team",5,10,this,1);
		mans[1].strength = Random.Range(1,SharedVars.Inst.charMax);
		mans[1].intellect = Random.Range(1,SharedVars.Inst.charMax);
		mans[1].social = Random.Range(1,SharedVars.Inst.charMax);
		soldierIndex = 1;
		mans[2] = new PointBase("medic","team",10,5,this,2);
		mans[2].strength = Random.Range(1,SharedVars.Inst.charMax);
		mans[2].intellect = Random.Range(1,SharedVars.Inst.charMax);
		mans[2].social = Random.Range(1,SharedVars.Inst.charMax);
		medicIndex = 2;
		for (int i=3;i<enemyNumber+3;i++)
		{
			mans[i] = new PointBase("enemy"+i.ToString(),"enemy",Random.Range(0.1f,0.9f)*SharedVars.Inst.operationMapSize,Random.Range(0.1f,0.9f)*SharedVars.Inst.operationMapSize,this,i);
			mans[i].startEndlessWalking();
		}

		for (int i=enemyNumber+3;i<enemyNumber+3+objectsNumber;i++)
		{
			mans[i] = new PointBase("object"+i.ToString(),"object",Random.Range(0.1f,0.9f)*SharedVars.Inst.operationMapSize,Random.Range(0.1f,0.9f)*SharedVars.Inst.operationMapSize,this,i);
		}
		mans[mans.Length-1] = new PointBase("mission","mission",Random.Range(0.75f,0.9f)*SharedVars.Inst.operationMapSize,Random.Range(0.75f,0.9f)*SharedVars.Inst.operationMapSize,this,mans.Length-1);
		missionIndex = mans.Length-1;
		allteamPoints = createPoints(allteamPoints,screen3);
		capitanPoints = createPoints(capitanPoints,screen0);
		soldierPoints = createPoints(soldierPoints,screen1);
		medicPoints = createPoints(medicPoints,screen2);

		mans[capitanIndex].selectFinalTarget(mans[mans.Length-1].positionX,mans[mans.Length-1].positionY);
		mans[soldierIndex].selectFinalTarget(mans[mans.Length-1].positionX,mans[mans.Length-1].positionY);
		mans[medicIndex].selectFinalTarget(mans[mans.Length-1].positionX,mans[mans.Length-1].positionY);

	}

	protected GameObject[] createPoints(GameObject[] currentMassive, GameObject currentScreen)
	{
		currentMassive = new GameObject[mans.Length];
		for (int j=0;j<mans.Length;j++)
		{
			GameObject neededClass = enemyPoint;
			switch (mans[j].fraction)
			{
			case "enemy":
				neededClass = enemyPoint; 
				
				break;
			case "object":
				neededClass = objectPoint; 				
				break;
			case "mission":
				neededClass = missionTargetPoint; 				
				break;
			case "team":
				
				if(mans[j].callsign == "capitan") neededClass = capitanPoint; 
				else if(mans[j].callsign == "soldier") neededClass = soldierPoint; 
				else if(mans[j].callsign == "medic") neededClass = medicPoint; 
				break;
			}
			currentMassive[j] = (GameObject)Instantiate(neededClass, new Vector3(currentScreen.transform.position.x+everyScreenWidth*(mans[j].positionX/SharedVars.Inst.operationMapSize),currentScreen.transform.position.y+everyScreenHeight*(mans[j].positionY/SharedVars.Inst.operationMapSize),0),neededClass.transform.rotation);
			if(currentScreen != screen3)
			{
				currentMassive[j].transform.localScale = currentMassive[j].transform.localScale*personalScreenScale/2;
			}
		}
		return currentMassive;
	}
	
	// Update is called once per frame
	void Update () {
		currentTime += Time.deltaTime;
		if(currentTime-lastTimeAlertsClean>0.25f)
		{
			screenAlert0.SetActive(false);
			screenAlert1.SetActive(false);
			screenAlert2.SetActive(false);
			lastTimeAlertsClean = currentTime;
		}
		moveTeam();
		moveOthers();
		updateScreen(screen3, allteamPoints, -1);
		updateScreen(screen0, capitanPoints, capitanIndex);
		updateScreen(screen1, soldierPoints, soldierIndex);
		updateScreen(screen2, medicPoints, medicIndex);

		if((mans[capitanIndex].health<=0) && (mans[soldierIndex].health<=0 ) && (mans[medicIndex].health<=0))
		{
			Debug.Log("Mission failed. Start again");
			Application.LoadLevel(0);
		}
	}


	protected void moveTeam()
	{
		mans[capitanIndex].Move();
		mans[soldierIndex].Move();
		mans[medicIndex].Move();
	}

	protected void moveOthers()
	{
		for (int j=0;j<mans.Length;j++)
		{
			if(mans[j].fraction =="enemy")
			{
				mans[j].Move();
			} else if (mans[j].fraction =="object")
			{
				mans[j].ObjectAction();
			}
		}
	}



	protected void updateScreen(GameObject currentScreen,GameObject[] currentMassive, int currentIndex = -1)
	{
		if(currentIndex==-1)
		{
			for (int j=0;j<mans.Length;j++)
			{
				//Debug.Log(j);
				//Debug.Log(currentMassive[j].transform.position);
				currentMassive[j].transform.position = new Vector3(currentScreen.transform.position.x+everyScreenWidth*(mans[j].positionX/SharedVars.Inst.operationMapSize),currentScreen.transform.position.y+everyScreenHeight*(mans[j].positionY/SharedVars.Inst.operationMapSize),0);
				if(mans[j].health<=0)
				{
					(currentMassive[j] as GameObject).SetActive(false);
				}

			}
		} else 
		{
			if( mans[currentIndex].health>0)
			{
				float coordXFix =currentScreen.transform.position.x+everyScreenWidth/2 - (currentScreen.transform.position.x+everyScreenWidth*(mans[currentIndex].positionX/SharedVars.Inst.operationMapSize));
				float coordYFix =currentScreen.transform.position.y+everyScreenHeight/2 - (currentScreen.transform.position.y+everyScreenHeight*(mans[currentIndex].positionY/SharedVars.Inst.operationMapSize));
				currentMassive[currentIndex].transform.position = new Vector3(currentScreen.transform.position.x+everyScreenWidth/2,currentScreen.transform.position.y+everyScreenHeight/2,0);
				//Debug.Log(coordXFix + ", "+ coordYFix);
				for (int j=0;j<mans.Length;j++)
				{
					if(j!=currentIndex)
					{
						//Debug.Log(j);
						//Debug.Log(currentMassive[j].transform.position);
						float unscaledPosX = currentScreen.transform.position.x+everyScreenWidth*(mans[j].positionX/SharedVars.Inst.operationMapSize)+coordXFix;
						float unscaledPosY = currentScreen.transform.position.y+everyScreenHeight*(mans[j].positionY/SharedVars.Inst.operationMapSize)+coordYFix;
						//currentMassive[j].transform.position = new Vector3(currentScreen.transform.position.x+everyScreenWidth*(mans[j].positionX/operationMapSize)+coordXFix,currentScreen.transform.position.y+everyScreenHeight*(mans[j].positionY/operationMapSize)+coordYFix,0);
						float perscoordXFix = currentMassive[currentIndex].transform.position.x - unscaledPosX;
						float perscoordYFix = currentMassive[currentIndex].transform.position.y - unscaledPosY;
						currentMassive[j].transform.position = new Vector3(unscaledPosX+perscoordXFix-perscoordXFix*personalScreenScale,unscaledPosY+perscoordYFix-perscoordYFix*personalScreenScale,0);

						if(currentMassive[j].transform.position.x<currentScreen.transform.position.x+0.2f || currentMassive[j].transform.position.x>currentScreen.transform.position.x+everyScreenWidth-0.2f ||
						   currentMassive[j].transform.position.y<currentScreen.transform.position.y+0.2f || currentMassive[j].transform.position.y>currentScreen.transform.position.y+everyScreenHeight-0.2f || mans[j].health<=0)
						{
							(currentMassive[j] as GameObject).SetActive(false);
						} else 
						{
							(currentMassive[j] as GameObject).SetActive(true);
						}
					}
				}
			} else 
			{
				for (int j=0;j<mans.Length;j++)
				{
					(currentMassive[j] as GameObject).SetActive(false);
				}
			}
		}
		//return currentMassive;
	}

	public void setSignal(int index, string command, string direction, AudioClip sndTeam=null, AudioClip sndCommand=null, AudioClip sndDirection=null)
	{
		if(sndDirection!=null)
		{
			Debug.Log("SET SIGNAL");
			StartCoroutine(playOneByOne(sndTeam,sndCommand,sndDirection));
			//playOneByOne(sndTeam,sndCommand,sndDirection);
		}
		StartCoroutine(resumeSignal(index, command, direction, sndTeam,sndCommand,sndDirection));
	}

	IEnumerator resumeSignal(int index, string command, string direction, AudioClip sndTeam=null, AudioClip sndCommand=null, AudioClip sndDirection=null)
	{
		yield return new WaitForSeconds((sndTeam.length+sndCommand.length + sndDirection.length)/1.5f);
		if((index==capitanIndex || index == soldierIndex || index == medicIndex) && mans[index].health>0)
		{
			mans[index].getSignal(command,direction);
			
		} else 
		{
			if (mans[capitanIndex].health>0) mans[capitanIndex].getSignal(command,direction);
			if (mans[soldierIndex].health>0) mans[soldierIndex].getSignal(command,direction);
			if (mans[medicIndex].health>0) mans[medicIndex].getSignal(command,direction);
		}
	}


	IEnumerator playOneByOne(params AudioClip[] values)
	{
		Debug.Log("playOneByOne");
		canalDispetcher.pitch = 1.5f;
		for(int i=0;i<values.Length;i++)
		{
			canalDispetcher.PlayOneShot(values[i]);
			yield return new WaitForSeconds(values[i].length/1.5f);
		}
		canalDispetcher.pitch = 1;
	}

	public void playOneSound(AudioClip value, PointBase unit)
	{
		AudioSource currentCanal = null;

		currentCanal = canalDispetcher;

		if(unit!=null) 
		{
			if(unit.massiveIndex == capitanIndex)
			{
				currentCanal = canalAlpha;
			} else if(unit.massiveIndex == soldierIndex)
			{
				currentCanal = canalDelta;
			} if(unit.massiveIndex == medicIndex)
			{
				currentCanal = canalOmega;
			}
			if(unit.lastSound!=value && !currentCanal.isPlaying) 
			{
				currentCanal.clip = value;
				currentCanal.Play();
				//currentCanal.PlayOneShot(value);
				//unit.lastSound = value;
			}
		} else if (!currentCanal.isPlaying)
		{
			currentCanal.clip = value;
			currentCanal.Play();
			//currentCanal.PlayOneShot(value);
		}
	}

	public void setAlertQuad(int index)
	{
		if(currentTime-lastTimeAlertsClean>0.10f)
		{
			if (index==capitanIndex)
			{
				screenAlert0.SetActive(true);
			} else if (index == soldierIndex)
			{
				screenAlert1.SetActive(true);
			} else if (index == medicIndex)
			{
				screenAlert2.SetActive(true);
			}
		}
	}

	public void missionComplete(int index)
	{
		if(!isExitMission)
		{
			mans[missionIndex].health = 0;
			Debug.Log(mans[index].callsign + " completed mission. EveryBody out!");
			mans[capitanIndex].selectFinalTarget(0f,0f);
			mans[soldierIndex].selectFinalTarget(0f,0f);
			mans[medicIndex].selectFinalTarget(0f,0f);
			isExitMission = true;
			playOneSound(snd_missioncomplete,null);
		} else 
		{
			Debug.Log(mans[index].callsign + " on board");
			mans[index].isOnBoard = true;
			if((mans[capitanIndex].health<=0 || mans[capitanIndex].isOnBoard) && (mans[soldierIndex].health<=0 || mans[soldierIndex].isOnBoard) && (mans[medicIndex].health<=0 || mans[medicIndex].isOnBoard))
			{
				Debug.Log("Mission ended. Let's go away!");
				Application.LoadLevel(0);
			}
		}
	}

	public PointBase GetUnit(int index)
	{
		return mans[index];
	}
}
