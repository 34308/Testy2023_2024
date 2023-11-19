import SplashScreen from 'react-native-splash-screen';
import React, { useEffect, useState } from "react";
import { MenuProvider } from 'react-native-popup-menu';

import loginReducer from './Components/Reducer';
import {Provider as StoreProvider} from 'react-redux';
import store from './Components/store';
import Navigation from './Components/Navigation';
import FlashMessage, { showMessage } from "react-native-flash-message";
import { checkIfServerActive } from "./Components/Utilities";
import { COLORS } from "./Components/Colors";

const SERVER_ACTIVE = 'active';
const SERVER_INACTIVE = 'inactive';
function App() {
    SplashScreen.hide();
    let checkingInterval = 5000;
    /*
    /

    const checkServerStatus = async () => {
      if (await checkIfServerActive()) {
        console.log('serwer aktywny');
      } else {
        console.log('serwer nieaktywny');
        showMessage({
          message: 'Brak połączenia z serwerem',
          type: 'info',
          backgroundColor: COLORS.second,
          color: COLORS.main,
        })
      }
    };
    */

    useEffect(() => {
      //const intervalId = setInterval(checkServerStatus, checkingInterval);
      //return () => clearInterval(intervalId);
    }, [])
    return (

      <StoreProvider store={store}>
          <MenuProvider>
              <Navigation/>
          </MenuProvider>
        <FlashMessage position="top" />
      </StoreProvider>

    );
  }

export default App;
