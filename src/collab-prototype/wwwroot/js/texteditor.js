var editor = document.getElementById("editor");

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/texthub")
    .build();

connection.start().catch(err => console.error(err));

connection.on("ReceiveText", (text) => {
    editor.value = text;
    editor.focus();
    editor.setSelectionRange(editor.value.length, editor.value.length);
});

function change() {
    connection.invoke("BroadcastText", editor.value).catch(err => console.error(err));
}

editor.style.display = "none";
var group = document.getElementById("group");
function join() {
    connection.invoke("JoinGroup", group.value).catch(err => console.error(err));
    editor.style.display = "initial";
}