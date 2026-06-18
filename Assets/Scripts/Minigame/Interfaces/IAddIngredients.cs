using UnityEngine;

public interface IAddIngredients
{
    public virtual void AddIngredientsToBowl() { }
    public virtual void AddIngredientsToBowl(float amount, string ingredientPassed) { }
}
