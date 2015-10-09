using UnityEngine;
using System.Collections;

public class PointBase  {

	public float positionX;
	public float positionY;
	public float angle;
	public float speed = 2;
	public string fraction;

	public float targetX;
	public float targetY;
	public float finalTargetX;
	public float finalTargetY;

	public string callsign;
	public float health=100;
	public float strength;
	public float intellect;
	public float social;

	public float bonusHealth=0;
	public float bonusStrength=0;
	public float bonusIntellect=0;
	public float bonusSocial=0;


	public bool isOnBoard = false;
	public string readyDirection = "";

	protected int targetsLeft = 0;
	protected bool isEndlessWalking = false;
	protected FightController game;
	public int massiveIndex;
	protected bool isObjectSearching = false;
	protected bool isObjectFound = false;

	public AudioClip lastSound = null;


	public PointBase(string sign, string fract, float posX, float posY, FightController gameInstance, int index=-1)
	{
		game = gameInstance;
		callsign = sign;
		fraction = fract;
		positionX = posX;
		positionY = posY;
		massiveIndex = index;
		if(fraction=="enemy")
		{
			strength = SharedVars.Inst.enemyStrength;
			intellect = SharedVars.Inst.enemyIntellect;
			social = SharedVars.Inst.enemySocial;
			health = SharedVars.Inst.enemyHealth;
		} else if(fraction == "object")
		{
			bonusStrength = Random.Range(0,10);
			bonusIntellect = Random.Range(0,10);
			bonusSocial = Random.Range(0,10);
			bonusHealth = Random.Range(0,10);
		}
	}

	public void selectFinalTarget(float targX, float targY)
	{
		finalTargetX = targX;
		finalTargetY = targY;
		if(targetsLeft<(SharedVars.Inst.charMax-SharedVars.Inst.charMax/5-intellect)/10)
		{
			targetX = Random.Range(0.1f,0.9f)*SharedVars.Inst.operationMapSize;
			targetY = Random.Range(0.1f,0.9f)*SharedVars.Inst.operationMapSize;
			targetsLeft++;
		} else 
		{
			targetX = targX;
			targetY = targY;
		}
	}

	public void selectTarget(float targX, float targY)
	{
			targetX = targX;
			targetY = targY;
	}

	public void startEndlessWalking()
	{
		isEndlessWalking = true;
		targetX = Random.Range(0.1f,0.9f)*SharedVars.Inst.operationMapSize;
		targetY = Random.Range(0.1f,0.9f)*SharedVars.Inst.operationMapSize;
	}

	public void Move()
	{
		if(health>0)
		{
			bool needMove = false;
			if(Mathf.Abs(positionX-targetX)>0.5f || Mathf.Abs(positionY-targetY)>0.5f)
			{
				needMove = true;
			}

			if(fraction=="enemy")
			{
				if(checkEncounter(game.GetUnit(0)) || checkEncounter(game.GetUnit(1)) || checkEncounter(game.GetUnit(2)))
				{
					needMove = false;
				}
				
			}
			//Debug.Log(needMove);
			if(needMove)
			{

				Vector2 currentPos = new Vector2(positionX,positionY);
				Vector2 targetPos = new Vector2(targetX,targetY);
				Vector2 direction = Vector2.MoveTowards(currentPos, targetPos, Time.deltaTime * speed);
				positionX = direction.x;
				positionY = direction.y;
			} else if(targetX!=finalTargetX || targetY!=finalTargetY) 
			{
				if(!isEndlessWalking) selectFinalTarget(finalTargetX, finalTargetY);
				else startEndlessWalking();
			} else if(!isOnBoard)
			{
				if(fraction =="team") game.missionComplete(massiveIndex);
			}


		}

	}

	public void ObjectAction()
	{
		if(health>0)
		{
			checkObjectEncounter(game.GetUnit(0));
			checkObjectEncounter(game.GetUnit(1));
			checkObjectEncounter(game.GetUnit(2));
		}
	}

	protected bool checkObjectEncounter(PointBase unit)
	{
		bool findUnit = false;
		float distance = checkDistance(this, unit);
		float intellectBonus = 1;
		if(unit.isObjectSearching)
		{
			intellectBonus = SharedVars.Inst.checkImprove;
		}
		if(distance<=intellectBonus*unit.intellect/10 && unit.health>0)
		{
			findUnit = true;
			health = 0;
			unit.FindObject(this);
		}
		
		return findUnit;
	}

	public void FindObject(PointBase obj)
	{
		isObjectFound = true;
		strength += obj.bonusStrength;
		intellect+= obj.bonusIntellect;
		social += obj.bonusSocial;
		health += obj.bonusHealth;
	}

	protected bool checkEncounter(PointBase unit)
	{
		bool findUnit = false;
		float distance = checkDistance(this, unit);
		string atackDirection = CheckAtackDirection(this,unit);
		if(distance<=intellect/10 && unit.health>0)
		{
			findUnit = true;
			if(unit.readyDirection=="")
			{
				unit.GetDamage(strength*Time.deltaTime);
			} else if(atackDirection == unit.readyDirection)
			{
				unit.MakeShot(this);

			} else 
			{
				unit.GetDamage(2*strength*Time.deltaTime);
			}

			selectTarget(unit.positionX, unit.positionY);
		}
		float distanceModificator = 1;
		if(atackDirection == unit.readyDirection)
		{
			distanceModificator = 2;
		} else if (unit.readyDirection!="")
		{
			distanceModificator = 0.5f;
		}
		if(distance<=distanceModificator*unit.intellect/10 && unit.health>0)
		{
			unit.MakeShot(this);
			selectTarget(unit.positionX, unit.positionY);
		}

		return findUnit;
	}

	public void MakeShot(PointBase target)
	{
		target.GetDamage(strength*Time.deltaTime);
		game.playOneSound(game.snd_enemycontact,this);
	}

	protected string CheckAtackDirection(PointBase A, PointBase B)
	{
		if(A.positionY>B.positionY)
		{
			if(A.positionY-B.positionY>=Mathf.Abs(A.positionX-B.positionY))
			{
				return "up";
			} else if(A.positionX>=B.positionX)
			{
				return "right";
			} else 
			{
				return "left";
			}
		} else 
		{
			if(B.positionY-A.positionY>=Mathf.Abs(A.positionX-B.positionY))
			{
				return "down";
			} else if(A.positionX>=B.positionX)
			{
				return "right";
			} else 
			{
				return "left";
			}
		}
	}
	
	public float checkDistance(PointBase A, PointBase B)
	{
		return Mathf.Sqrt((A.positionX - B.positionX) * (A.positionX - B.positionX) + (A.positionY - B.positionY) * (A.positionY - B.positionY));
	}

	public void GetDamage(float points)
	{
		health-=points;
		game.setAlertQuad(massiveIndex);
		if(health<=0)
		{
			health = 0;
		} else if (fraction=="team")
		{
			game.playOneSound(game.snd_injured,this);
		}
	}

	public void getSignal(string command, string direction)
	{
		switch (command)
		{
		case "check":
			if (social>=20)
			{
				setPointToCheck(direction);
				game.playOneSound(game.snd_roger,this);
			} else 
			{
				game.playOneSound(game.snd_screw,this);
			}
			break;
		case "danger":
			if (social>=20)
			{
				getReadyToFight(direction);
				game.playOneSound(game.snd_roger,this);
			} else 
			{
				game.playOneSound(game.snd_screw,this);
			}
			break;
		}
	}

	protected void getReadyToFight(string direction)
	{
		readyDirection = direction;
		game.StartCoroutine(this.getRelaxed());
	}

	IEnumerator getRelaxed()
	{
		yield return new WaitForSeconds(3f);
		readyDirection = "";
		game.playOneSound(game.snd_clear,this);
	}

	protected void setPointToCheck(string direction)
	{
		switch (direction)
		{
		case "up":
			float newY = positionY+SharedVars.Inst.checkDistance;
			if(newY>SharedVars.Inst.operationMapSize) newY = SharedVars.Inst.operationMapSize;
			selectTarget(positionX, newY);

			break;
		case "down":
			newY = positionY-SharedVars.Inst.checkDistance;
			if(newY<0) newY = 0;
			selectTarget(positionX, newY);
			break;
		case "right":
			newY = positionX+SharedVars.Inst.checkDistance;
			if(newY>SharedVars.Inst.operationMapSize) newY = SharedVars.Inst.operationMapSize;
			selectTarget(newY, positionY);
			break;
		case "left":
			newY = positionX-SharedVars.Inst.checkDistance;
			if(newY<0) newY = 0;
			selectTarget(newY, positionY);
			break;
		}
	}
}
