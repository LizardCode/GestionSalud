namespace LizardCode.Framework.ReneBusClient.Responses.Training
{
    public class RepositoryGetImagesByRepositoryResponse
    {
        public int IdImage { get; set; }
        public string Filename { get; set; }
        public string Format { get; set; }
        public string Path { get; set; }
        public bool IsTrainFolder { get; set; }
        public DateTime DateUpload { get; set; }
        public IList<RepositoryGetAnnotationByRepositoryResponse> Annotations { get; set; }
    }
}
