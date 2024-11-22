using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementGranter : ButtonPress
{

    [SerializeField] int achievementToGrant;

    override protected void Start() {
        onPress(() => {
            Settings.setAchievementUnlocked(achievementToGrant);
        });
    }

}
