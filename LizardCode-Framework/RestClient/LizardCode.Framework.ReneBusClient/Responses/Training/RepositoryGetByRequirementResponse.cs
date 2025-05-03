namespace LizardCode.Framework.ReneBusClient.Responses.Training
{
    public class RepositoryGetByRequirementResponse
    {
        public int IdRepository { get; set; }
        public string Description { get; set; }
        public int IdTypeRepository { get; set; }
        public string TypeRepository { get; set; }
        public int IdSubTypeRepository { get; set; }
        public string SubTypeRepository { get; set; }
        public int IdStateRepository { get; set; }
        public string StateRepository { get; set; }
    }
}
