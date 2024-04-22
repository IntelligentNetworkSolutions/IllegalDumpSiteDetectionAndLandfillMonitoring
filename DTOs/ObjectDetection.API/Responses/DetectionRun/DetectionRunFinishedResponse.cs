namespace DTOs.ObjectDetection.API.Responses.DetectionRun
{
    public class DetectionRunFinishedResponse
    {
        public int[] labels { get; set; }
        public float[] scores { get; set; }
        public float[][] bboxes { get; set; }
    }


}
