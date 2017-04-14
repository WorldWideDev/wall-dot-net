using System.Collections.Generic;

namespace Wall.Models
{
    public class WallModelBundle
    {
        public WallUser UserModel { get; set; }
        public List<Message> AllPosts {get; set; }
        public List<Comment> AllComments {get; set; }

        public Message NewMessage {get; set; }
        public Comment NewComment {get; set; }
    }

}