import './css/signin.css';
import { isEmpty, fireEvent } from './js/utils';

const clear = (e) => document.getElementById('msg').innerHTML = '';

window.addEventListener('keyup', (e) => {
    if (e.keyCode !== 13)
        return;
    fireEvent(document.getElementById('submitBtn'), 'click');
});

document.getElementById('submitBtn').addEventListener('click', (e) => {
    e.preventDefault();
    var uid = document.forms.signin.uid.value,
        pwd = document.forms.signin.pwd.value,
        url = /\?returnurl=(\/[\w/]{0,})/i.exec(decodeURIComponent(location.search));
    if (isEmpty(uid) || isEmpty(pwd))
        return;
    if (url && url.length > 1)
        url = url[1];
    $.ajax({
        url: '/api/v1/account/signin',
        method: 'POST',
        data: {
            n: document.forms.signin.uid.value,
            p: btoa(document.forms.signin.pwd.value),
            url: url
        }
    })
    .done(msg => location.href = msg.url)
    .fail(err => {
        document.getElementById('msg').innerHTML='帳號或密碼錯誤'
    });
});
document.getElementById('uid').addEventListener('focus', clear);
document.getElementById('pwd').addEventListener('focus', clear);

