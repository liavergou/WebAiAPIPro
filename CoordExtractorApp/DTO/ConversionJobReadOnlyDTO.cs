using CoordExtractorApp.Core.Enums;

namespace CoordExtractorApp.DTO
{
    /// <summary>
    /// Data Transfer Object for returning conversion job details.
    /// </summary>
    public class ConversionJobReadOnlyDTO
    {
        /// <summary>
        /// The unique identifier of the conversion job.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the uploaded file.
        /// </summary>
        public string OriginalFileName { get; set; } = null!;

        /// <summary>
        /// The name of the processed/cropped file.
        /// </summary>
        public string? CroppedFileName { get; set; }

        /// <summary>
        /// The name of the AI model used for processing.
        /// </summary>
        public string? ModelUsed { get; set; }

        /// <summary>
        /// The list of coordinates extracted from the image.
        /// </summary>
        public List<CoordinateDTO> Coordinates { get; set; } = new();

        /// <summary>
        /// The current status of the job (Pending, Success, Failed).
        /// </summary>
        public JobStatus Status { get; set; } = JobStatus.Pending;

        /// <summary>
        /// Any error message generated during processing.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// The ID of the project associated with this job.
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// The ID of the prompt used for this job.
        /// </summary>
        public int PromptId { get; set; }

        /// <summary>
        /// The timestamp when the job was soft-deleted, if applicable.
        /// </summary>
        public DateTime? DeletedAt { get; set; }



    }
}
