import jwtDecode from 'jwt-decode';
import { API_URL, LOGOUT } from "./actions";
import store from './store';
import {showMessage} from 'react-native-flash-message';
import {COLORS} from './Colors';
import { re } from "@babel/core/lib/vendor/import-meta-resolve";

export function isTokenExp(token) {
  return jwtDecode(token).exp < Date.now() / 1000;
}
export function getUserName(token) {
  return jwtDecode(token).sub;
}
export function LogOut(navigation, dispatch) {
  dispatch({
    type: LOGOUT,
  });
  navigation.goBack();
}
export function checkIfLoggedAndLogout(navigation, store) {
  fetch(API_URL + '/islogged', {
    method: 'get',
    headers: {Authorization: 'Bearer ' + store.getState().token},
  }).then(response => {
    if (!response.ok) {
      showMessage({
        message: 'Twoja sesja wygasła zaloguj się ponownie.',
        type: 'info',
        backgroundColor: COLORS.second,
        color: COLORS.main,
      });
      LogOut(navigation, store.dispatch);
      navigation.navigate('Citys');
      return true;
    }
  });
}
export async function checkIfLogged() {
  const response = await fetch(API_URL + 'User/islogged', {
    method: 'get',
    headers: {Authorization: store.getState().token},
  });
  if (!response.ok) {
    showMessage({
      message: 'Twoja sesja wygasła zaloguj się ponownie.',
      type: 'info',
      backgroundColor: COLORS.second,
      color: COLORS.main,
    });
    return false;
  }else{
    return true;
  }
}

export async function checkIfServerActive() {
  const httpTimeout = 1000;
  const controller = new AbortController();
  const timeoutId = setTimeout(() => controller.abort(), httpTimeout);
  try {
    const response = await fetch(API_URL + '/activity', {signal: controller.signal})
    if (response.status === 200) {
      return true
    } else {
      return false
    }
  } catch (error) {
    return false
  }
}

export function validate(text) {
  let reg = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w\w+)+$/;
  if (reg.test(text) === false) {
      return false;
  }
  else {
    console.log("Email is Correct");
  }
}
