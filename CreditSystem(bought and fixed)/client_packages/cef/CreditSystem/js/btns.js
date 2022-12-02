function galocka(){ 
    let a = document.getElementById("check1").value;
    let b = document.getElementById("check2").value;

    
    if (a >= 0){
        document.getElementById("check_mark1").innerHTML ='<div class="galkaFalse"</div>';
    } else {
        document.getElementById("check_mark1").innerHTML = '<div class="galkaTrue"</div>';
    } 
    if (b >= 0){
        document.getElementById("check_mark2").innerHTML ='<div class="galkaFalse"</div>';
    } else {
        document.getElementById("check_mark2").innerHTML = '<div class="galkaTrue"</div>';
    }
    switch(type){
        case 0:
                document.getElementById("check_mark3").innerHTML ='<div class="galkaFalse"</div>';
            break;
        case 1:
            document.getElementById("check_mark3").innerHTML ='<div class="galkaTrue"</div>';
            break;
    }
}