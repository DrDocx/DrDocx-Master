function submitForm() {
    for (let field of document.querySelectorAll(":disabled")) {
        field.disabled = false;
    }
  document.getElementById("form-submit").click();
}

function toggleEdit(buttonId, fieldIds) {
  let butt = document.getElementById(buttonId);
  butt.innerText = "Save";
  for(let fieldId of fieldIds) {
    let field = document.getElementById(fieldId);
    field.parentElement.MaterialTextfield.enable();
  }
  butt.onclick = submitForm;
}

function editTable(buttonId, rowSelector) {
  let butt = document.getElementById(buttonId);
  butt.innerText = "Save";
  for(let row of document.querySelector(rowSelector)) {
    let raw = row.childNodes[1];
    let scaled = row.childNodes[2];
  }
  butt.onclick = submitForm;
}

window.addEventListener("load", function () {
    let tabs = document.getElementById("tabs").children;
    for (let i = 0; i < tabs.length; i++) {
        tabs[i].addEventListener("click", function () {
            document.cookie = "tab=" + i;
        });
    }
    let goalTab = parseInt(getCookie("tab"));
    console.log(goalTab);
    console.log(tabs);
    console.log(tabs[goalTab]);
    if (goalTab) {
        tabs[goalTab].click();
    }
});

function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

