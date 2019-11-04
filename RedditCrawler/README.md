# RedditCrawler

I hypothesize that most Reddit posts are connected through only links that stay on Reddit. This crawler searches each Reddit page for links containing `r/.../comments/...` or `user/...` and crawls through them, maintaining a list of URLs previously visited. At the end, unique usernames and Reddit posts are dumped to a text file.

## Topics Employed:
- HTTPClient
- Async Tasks
- File I/O
