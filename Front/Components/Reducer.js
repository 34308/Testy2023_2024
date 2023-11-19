import {getData, storeData} from './StorageHelper';
import {LOGIN, LOGOUT,DISCARDAVATAR,SETAVATAR,SETPINS,DELETEPINS} from './actions';

const token = getData('JWT');
const Avatar= getData('Avatar');
const Pin =getData('Pin')
let initialState;

if (token) {
  console.log('A1')
  if(Pin){

    if(Avatar){
      initialState = {isLoggedIn: false, token,Pin,Avatar};
    }else{
      initialState = {isLoggedIn: false, token,Pin};
    }
  }else{
    console.log('A3')
    initialState = {isLoggedIn: false, token};
  }


} else {
  initialState = {isLoggedIn: false};
}

export default function loginReducer(state = initialState, action) {
  const {type, payload} = action;
  switch (type) {
    case LOGIN:
      storeData('JWT', payload);
      return {
        ...state,
        token: payload,
        isLoggedIn: true,
      };
    case LOGOUT:
      storeData('JWT', '');
      storeData('Avatar', '');
      return {
        ...state,
        user: '',
        Avatar: '',
        isLoggedIn: false,
      };
    case SETAVATAR:
      storeData('Avatar', payload);
      return {
        ...state,
        Avatar: payload,
      };
    case DISCARDAVATAR:
      storeData('Avatar', '');
      return {
        ...state,
        Avatar: '',
      };
    case SETPINS:
      storeData('Pin', payload);
      return {
        ...state,
        Pin: payload,
      };
    case DELETEPINS:
      storeData('Pin', '');
      return {
        ...state,
        Pin: '',
      };
    default:
      return state;
  }
}

