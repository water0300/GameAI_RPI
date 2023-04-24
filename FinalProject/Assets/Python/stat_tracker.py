import UnityEngine as UE

objects = UE.Object.FindObjectsOfType(UE.GameObject)
for go in objects:
	UE.Debug.Log(go.name)