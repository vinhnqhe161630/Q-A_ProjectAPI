﻿namespace PRN231_ProjectQA_MVC.Models.Comment
{
    public class AddAnswerCommentmodel
    {
       
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
        public Guid CommentId { get; set; }

    }
}
