using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wall.Models;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;

namespace Wall.Controllers
{
    public class WallController : Controller
    {
        private readonly DbConnector _dbConnector;
        public WallController(DbConnector connect)
        {
            _dbConnector = connect;
        }

        [HttpGetAttribute]
        [RouteAttribute("wall")]
        public IActionResult Index()
        {
            if(HttpContext.Session.GetInt32("id") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            // get session id for active user and db queries
            int id = (int)HttpContext.Session.GetInt32("id");
            WallModelBundle bundle = GetWallBundle(id);
            return View(bundle);
        }

        [HttpGetAttribute]
        [RouteAttribute("delete/{postId}")]
        public IActionResult DeleteWallPost(int postId)
        {
            int id = (int)HttpContext.Session.GetInt32("id");

            // only delete if post userid matches session user id
            if(PostUserIsValid(postId))
            {
                string deleteQuery = $@"DELETE FROM comments WHERE post_id = {postId};
                                        DELETE FROM posts WHERE id = {postId}";
                _dbConnector.Execute(deleteQuery);
            } else { System.Console.WriteLine("NAIUGHT NAUGHTY"); }

            WallModelBundle bundle = GetWallBundle(id);
            return RedirectToAction("Index");
        }

        [HttpPostAttribute]
        [RouteAttribute("posting")]
        public IActionResult NewPost(Message newMessage)
        {
            System.Console.WriteLine(newMessage);
            string newPostQuery = $@"INSERT INTO posts (post_content, user_id, created_at, updated_at)
                                     VALUES ('{newMessage.Content}', '{newMessage.UserId}', NOW(), NOW())";
            if(ModelState.IsValid)
            {
                _dbConnector.Execute(newPostQuery);
                return RedirectToAction("Index");
            }
            else 
            { 
                int id = (int)HttpContext.Session.GetInt32("id");
                WallModelBundle bundle = GetWallBundle(id);
                return View("Index", bundle); 
            }
        }

        [HttpPostAttribute]
        [RouteAttribute("commenting")]
        public IActionResult NewComment(Comment newComment)
        {
            System.Console.WriteLine(newComment);
            string newCommentQuery = $@"INSERT INTO comments (comment_content, user_id, post_id, created_at, updated_at)
                                     VALUES ('{newComment.Content}', '{newComment.UserId}', '{newComment.MessageId}', NOW(), NOW())";
            if(ModelState.IsValid)
            {
                _dbConnector.Execute(newCommentQuery);
                return RedirectToAction("Index");
            }
            else 
            { 
                int id = (int)HttpContext.Session.GetInt32("id");
                WallModelBundle bundle = GetWallBundle(id);
                return View("Index", bundle); 
            }
        }

        bool PostUserIsValid(int postId)
        {
            // check post's user id against session's user id
            int sessId = (int)HttpContext.Session.GetInt32("id");
            string postUserIdQuery = $"SELECT user_id FROM posts WHERE id = {postId}";
            List<Dictionary<string, object>> postToCheck = _dbConnector.Query(postUserIdQuery);
            if((int)postToCheck[0]["user_id"] == sessId){ return true; }
            return false;
        }

        WallModelBundle GetWallBundle (int sessionId)
        {
            List<Dictionary<string, object>> query = _dbConnector.Query($"SELECT first_name, last_name, id FROM users WHERE id = {sessionId}");
            string allPostsQuery = @"SELECT posts.*, users.first_name, users.last_name FROM posts
                                    LEFT JOIN users ON users.id = posts.user_id
                                    ORDER BY created_at DESC";
            List<Dictionary<string, object>> AllPosts = _dbConnector.Query(allPostsQuery);

            string allCommentsQuery = @"SELECT comments.*, users.first_name, users.last_name FROM comments
                    LEFT JOIN users ON users.id = comments.user_id
                    LEFT JOIN posts ON posts.id = comments.post_id";
            List<Dictionary<string, object>> AllComments = _dbConnector.Query(allCommentsQuery);

            // Initialize Models going to view
            List<Message> Messages = AllPostModels(AllPosts);
            List<Comment> Comments = AllCommentModels(AllComments);
            WallUser theWallUser = new WallUser
            {
                FirstName = (string)query[0]["first_name"],
                LastName = (string)query[0]["first_name"],
                ID = (int)query[0]["id"],
            };
            Message theMessage = new Message
            {
                UserId = theWallUser.ID
            };
            Comment theComment = new Comment
            {
                UserId = theWallUser.ID
            };
            return new WallModelBundle 
            { 
                AllPosts = Messages, 
                AllComments = Comments, 
                UserModel = theWallUser, 
                NewMessage = theMessage, 
                NewComment = theComment 
            };
        }

        List<Comment> AllCommentModels (List<Dictionary<string, object>> dbResult)
        {
            List<Comment> comments = new List<Comment>();
            foreach(var comment in dbResult)
            {
                System.Console.WriteLine(comment);
                Comment newComment = new Comment();
                foreach(KeyValuePair<string, object> col in comment)
                {
                    switch(col.Key)
                    {
                        case "id":
                            newComment.ID = (int)col.Value;
                            break;
                        case "comment_content":
                            newComment.Content = (string)col.Value;
                            break;
                        case "first_name":
                            newComment.FirstName = (string)col.Value;
                            break;
                        case "last_name":
                            newComment.LastName = (string)col.Value;
                            break;
                        case "created_at":
                            newComment.CreatedAt = (DateTime)col.Value;
                            break;
                        case "updated_at":
                            newComment.UpdatedAt = (DateTime)col.Value;
                            break;
                        case "user_id":
                            newComment.UserId = (int)col.Value;
                            break;
                         case "post_id":
                            newComment.MessageId = (int)col.Value;
                            break;
                    }
                }
                comments.Add(newComment);
            }
            return comments;
        }

        List<Message> AllPostModels (List<Dictionary<string, object>> dbResult)
        {
            List<Message> messages = new List<Message>();
            System.Console.WriteLine(dbResult);
            foreach(var post in dbResult)
            {
                System.Console.WriteLine(post);
                Message newMessage = new Message();
                foreach(KeyValuePair<string, object> col in post)
                {
                    switch(col.Key)
                    {
                        case "id":
                            newMessage.ID = (int)col.Value;
                            break;
                        case "post_content":
                            newMessage.Content = (string)col.Value;
                            break;
                        case "first_name":
                            newMessage.FirstName = (string)col.Value;
                            break;
                        case "last_name":
                            newMessage.LastName = (string)col.Value;
                            break;
                        case "created_at":
                            newMessage.CreatedAt = (DateTime)col.Value;
                            break;
                        case "updated_at":
                            newMessage.UpdatedAt = (DateTime)col.Value;
                            break;
                        case "user_id":
                            newMessage.UserId = (int)col.Value;
                            break;
                    }
                }
                messages.Add(newMessage);
            }
            return messages;
        }
    }
}