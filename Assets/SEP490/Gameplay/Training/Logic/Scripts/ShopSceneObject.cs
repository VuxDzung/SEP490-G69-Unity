namespace SEP490G69.Economy
{
    public class ShopSceneObject : BaseSceneObject
    {
        public override void Interact()
        {
            base.Interact();
            GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_SHOP);
        }
    }
}