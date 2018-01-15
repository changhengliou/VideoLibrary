import './css/signin.css';
import { isEmpty } from './js/utils';

document.getElementById('submitBtn').addEventListener('click', (e) => {
    e.preventDefault();
    var uid = document.forms.signin.uid.value,
        pwd = document.forms.signin.pwd.value;
    
    if (isEmpty(uid) || isEmpty(pwd))
        return;

    $.ajax({
        url: '',
        method: 'POST',
        data: {
            uid: document.forms.signin.uid.value,
            pwd: document.forms.signin.pwd.value
        }
    })
    .done(msg => console.log(msg))
    .fail(err => console.log(err))
});