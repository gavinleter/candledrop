using UnityEngine;

public enum specialObjectType {

    blackHole,
    miniSun

}

public interface ISpecialObject {

    public void destroySelf();
    public void setup(GameManager g, int id);
    //each speical object has a type associated with it, uses the enum above
    //used for the save manager
    public int getType();
    public GameObject getGameObject();

}
