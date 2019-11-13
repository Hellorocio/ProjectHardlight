using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class AbilityDetail : MonoBehaviour
{
    public TextMeshProUGUI abilityName;
    public TextMeshProUGUI abilityDesc;
    // TODO change to more dynamic things
    public TextMeshProUGUI damage;
    public TextMeshProUGUI sunlightName;
    public TextMeshProUGUI moonlightName;
    public TextMeshProUGUI starlightName;

    public Image sunlightIcon;
    public Image moonlightIcon;
    public Image starlightIcon;
    
    private Color fullOpacity = Color.white;
    private Color halfOpacity = new Color(Color.white.r, Color.white.g, Color.white.b, 0.5f);

    public void SetAbility(Ability ability)
    {
        abilityName.text = ability.abilityName;
        abilityDesc.text = ability.abilityDescription;
        damage.text = ability.GetDamage().ToString();
        sunlightName.text = ability.sunlightAugment.augmentTitle;
        moonlightName.text = ability.moonlightAugment.augmentTitle;
        starlightName.text = ability.starlightAugment.augmentTitle;
    }

    public void UpdateSoul(Soul soul)
    {
        Color sunColor = soul.GetAllightValue(AllightType.SUNLIGHT) > 0 ? fullOpacity : halfOpacity;
        sunlightName.color = sunColor;
        sunlightIcon.color = sunColor;
        
        Color moonColor = soul.GetAllightValue(AllightType.MOONLIGHT) > 0 ? fullOpacity : halfOpacity;
        moonlightName.color = moonColor;
        moonlightIcon.color = moonColor;
        
        Color starColor = soul.GetAllightValue(AllightType.STARLIGHT) > 0 ? fullOpacity : halfOpacity;
        starlightName.color = starColor;
        starlightIcon.color = starColor;
    }
}
