const hubConnection = new signalR.HubConnectionBuilder().withUrl("/comments").build();
const taskIdInp = document.getElementById("taskIdInput").value;
let commentsArea = document.getElementById("CommentsArea");

function generateComment(commentId, userName, body, creationDate, likesCount, dislikesCount) {
    let card = document.createElement('div');
    card.className = 'card comment mt-1';
    card.id = commentId;

    let cardBody = document.createElement('div');
    cardBody.className = 'card-body';

    let title = document.createElement('h6');
    title.innerText = userName;
    title.className = 'card-title';
    
    let created = document.createElement('span');
    created.innerText = creationDate;
    created.className = 'float-right';
    
    let likeBtn = document.createElement('button');
    likeBtn.innerText = 'Like';
    likeBtn.className = 'btn btn-danger btn-sm float-right comment-like';
    likeBtn.setAttribute("id", "comment-like-"+commentId);
    likeBtn.setAttribute("name", userName);

    let dislikeBtn = document.createElement('button');
    dislikeBtn.innerText = 'Dislike';
    dislikeBtn.className = 'btn btn-dark btn-sm float-right ml-1 comment-dislike-';
    dislikeBtn.setAttribute("id", "comment-dislike-"+commentId);

    let commentText = document.createElement('div');
    commentText.innerText = body;
    commentText.className = 'card-comment-text';
    
    cardBody.appendChild(created);
    cardBody.appendChild(title);
    cardBody.appendChild(commentText);
    cardBody.appendChild(dislikeBtn);
    cardBody.appendChild(likeBtn);
    card.appendChild(cardBody);
    
    return card;
}

let likeBtns = document.querySelectorAll('*[id^="comment-like"]');
likeBtns.forEach(item => {
    item.addEventListener("click", function(event) {
        let commentId = item.id.replace('comment-like-', '');
        hubConnection.invoke("LikeComment", commentId).catch(function(err) {
            return console.error(err.toString());
        });
        event.preventDefault();
    }); 
});

let dislikeBtns = document.querySelectorAll('*[id^="comment-dislike"]');
dislikeBtns.forEach(item => {
    item.addEventListener("click", function(event) {
        let commentId = item.id.replace('comment-dislike-', '');
        hubConnection.invoke("DislikeComment", commentId).catch(function(err) {
            return console.error(err.toString());
        });
        event.preventDefault();
    });
});

hubConnection.on("ReceiveComment", function (userName, userId, body, created, taskId, commentId) {
    if (taskId == document.getElementById("taskIdInput").value) {
        commentsArea.prepend(generateComment(commentId, userName, body, created, '0', '0'));
        likeBtn = document.getElementById("comment-like-"+commentId);
        likeBtn.addEventListener("click", function(event) {
            hubConnection.invoke("LikeComment", commentId.toString()).catch(function(err) {
                return console.error(err.toString());
            });
        });
        like.innerHTML = '<span class="text-white"><i class="fa fa-heart"></i>Like ' +'</span>'
    }
});
hubConnection.on("ReceiveLike", function(commentId, commentLikesCount, userId, hasLiked) {
    let userIdInput = document.getElementById("userIdInput").value;
    let like = document.getElementById("comment-like-"+commentId.toString());
    if(userIdInput == userId) {
        if(hasLiked) {
            like.innerHTML = '<strong class="text-white"><i class="fa fa-heart"></i>Liked '+ commentLikesCount +'</strong>'
        }else {
            like.innerHTML = 'Like '+ commentLikesCount +''
        }   
    }else {
        like.innerHTML = 'Like '+ commentLikesCount +''
    }
});

hubConnection.on("ReceiveDislike", function(commentId, commentDislikesCount, userId, hasDisliked) {
    let userIdInput = document.getElementById("userIdInput").value;
    let dislike = document.getElementById("comment-dislike-"+commentId.toString());
    if(userIdInput == userId) {
        if(hasDisliked) {
            dislike.innerHTML = '<strong class="text-white">Disliked '+ commentDislikesCount +'</strong>'
        }else {
            dislike.innerHTML = 'Dislike '+ commentDislikesCount +''
        }
    }else {
        dislike.innerHTML = 'Dislike '+ commentDislikesCount +''
    }
});

hubConnection.on("DislikeComment", function(commentId, commentLikesCount) {
    let like = document.getElementById("comment-like-"+commentId);
    like.innerText = 'Like '+commentLikesCount;
});

hubConnection.on("RemoveDislikeComment", function(commentId, commentDislikesCount) {
    let dislike = document.getElementById("comment-dislike-"+commentId);
    dislike.innerHTML = 'Dislike '+ commentDislikesCount +'';
});


document.getElementById("SendCommentBtn").addEventListener('click', (e) => {
    let message = document.getElementById("commentBody").value;
    hubConnection.invoke("AddComment", taskIdInp, message)
        .catch(function(err) {
        return console.error(err.toString());
    });
});
hubConnection.start();