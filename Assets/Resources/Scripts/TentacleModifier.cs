using UnityEngine;
using System.Collections;

public class TentacleModifier : MonoBehaviour 
{
    public float HingeMinMax = 45;

	// Use this for initialization
	void Start () 
    {
        ApplyAllChanges();
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    private void ApplyAllChanges()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (gameObject.transform.GetChild(i).GetComponent<HingeJoint>() != null)
            {
                JointLimits newLimit = gameObject.transform.GetChild(i).GetComponent<HingeJoint>().limits;
                newLimit.min = -HingeMinMax;
                newLimit.max = HingeMinMax;

                gameObject.transform.GetChild(i).GetComponent<HingeJoint>().limits = newLimit;
            }
        }
    }
}
