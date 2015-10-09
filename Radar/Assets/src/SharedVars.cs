using System.Collections;

class SharedVars
{
	
	public static SharedVars Inst = new SharedVars();

	public float operationMapSize = 100;
	public int charMax = 100;
	public float checkDistance = 10f;
	public float checkImprove = 2f;

	public int enemyStrength = 50;
	public int enemyIntellect = 50;
	public int enemySocial = 20;
	public float enemyHealth = 20;

	public string callsign0 = "Alpha Unit";
	public string callsign1 = "Delta Unit";
	public string callsign2 = "Omega Unit";
	public string callsign3 = "All Team";


}
