import React, { FC, useEffect, useState } from 'react';
import {
  Button,
  Image,
  StyleSheet,
  Text,
  TextInput,
  TouchableOpacity,
  View,
  ScrollView
} from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { COLORS } from '../Colors';
import NetInfo from '@react-native-community/netinfo';
import { API_URL, JWT, LOGIN, NOINTERNET, SERVER_ERROR } from '../actions';
import { showMessage } from 'react-native-flash-message';
import axios from "axios";


interface Avatar {
  Id: number;
  Picture: string;
}

export  default function Registration (navigation:any){
  const [login, setLogin] = useState<string>('');
  const [password, setPassword] = useState<string>('');
  const [passwordRepeat, setPasswordRepeat] = useState<string>('');
  const [email, setEmail] = useState<string>('');
  const [emailCorrect, setEmailCorrect] = useState<boolean>(false);
  const [Avatars, setAvatars] = useState<Avatar[]>([]);
  const [ChosenAvatar, setChosenAvatar] = useState<Avatar | undefined>();
  const [test, setTest] = useState<boolean>(false);

  useEffect(() => {
    const getAvatars = () => {
      axios.get(API_URL + "User/getAllBasicAvatars").then(result => {
        setAvatars(result.data.Data);
      })
    }
    getAvatars()
  }, []);

  const GoToLogin = () => {
    navigation.navigate('Login');
    setLogin('');
    setPassword('');
    setPasswordRepeat('');
    setEmail('');
  };

  const ValidateFields = async () => {
    if (
        login == '' ||
        password == '' ||
        passwordRepeat == '' ||
        email == ''
    ) {
      showMessage({
        message: 'Uzupełnij wszystkie pola.',
        type: 'warning',
        backgroundColor: COLORS.second,
        color: COLORS.main,
      });
    } else if (password.length < 8 || password.length > 20) {
      showMessage({
        message: 'Hasło musi zawierać min. 8 znaków i max. 20',
        type: 'warning',
        backgroundColor: COLORS.second,
        color: COLORS.main,
      });
    } else if (passwordRepeat !== password) {
      showMessage({
        message: 'Hasła się nie zgadzają.',
        type: 'warning',
        backgroundColor: COLORS.second,
        color: COLORS.main,
      });

    } else if (!emailCorrect) {
      showMessage({
        message: 'Niepoprawny email!',
        type: 'warning',
        backgroundColor: COLORS.second,
        color: COLORS.main,
      });
    } else {
      try {
        Register();
      } catch (error) {
        console.error(error);
      }
    }
  };

  async function Register() {
    try {
      axios.post(API_URL + 'User/SignUp', {
        login: login.toString(),
        email: email.toString(),
        password: password.toString(),
        avatarId: ChosenAvatar?.Id,
      }).then((result) => {

        if (result.data.Status != 0) {
          showMessage({
            message: '' + result.data.Message,
            type: 'info',
            backgroundColor: COLORS.mainOrange,
            color: 'black',
          });
        } else {
          showMessage({
            message: 'Pomyślnie utworzono konto.',
            type: 'info',
            backgroundColor: COLORS.mainOrange,
            color: 'black',
          });

          GoToLogin();
        }
      });

    } catch (error) {
      console.error("error:" + error);
    }
  }

  function ValidateEmail(text: string) {
    let reg = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w\w+)+$/;
    setEmail(text);
    setEmailCorrect(reg.test(text));
  }

  return (
      <ScrollView>
        <View>
          <View style={styles.container}>
            <View style={styles.section}>
              <View style={styles.logoContainer}>
                <View style={styles.column}>
                  <Text style={styles.textWelcome}>Zarejestruj się!</Text>
                </View>
              </View>
              <View style={styles.box}>
                <View>
                  <Text style={styles.textInput}>Login</Text>
                  <TextInput
                      style={styles.input}
                      value={login}
                      onChangeText={setLogin}
                      keyboardType="default"
                  />
                  <Text style={styles.textInput}>Hasło</Text>
                  <TextInput
                      value={password}
                      secureTextEntry={true}
                      onChangeText={setPassword}
                      style={styles.input}
                  />
                  <Text style={styles.textInput}>Powtórz Hasło</Text>
                  <TextInput
                      value={passwordRepeat}
                      secureTextEntry={true}
                      onChangeText={setPasswordRepeat}
                      style={styles.input}
                  />
                  <Text style={styles.textInput}>Email</Text>
                  <TextInput
                      value={email}
                      onChangeText={ValidateEmail}
                      style={styles.input}
                  />
                  {emailCorrect === false && (
                      <Text style={styles.wrongMail}>Nieprawidłowy Email</Text>
                  )}
                </View>
                <Text>Select Your Avatar</Text>
                <View style={styles.row}>
                  {Avatars ?
                      Avatars.map((item, i) => {
                        return (
                            <View key={i} >
                              <TouchableOpacity
                                  onPress={() => { setChosenAvatar(item) }}
                                  style={ChosenAvatar === item ? styles.selected : { margin: 5 }}
                              >
                                <Image
                                    style={{
                                      height: 100,
                                      width: 100,
                                      marginBottom: 10,
                                    }}
                                    source={{ uri: item.Picture }}
                                />
                              </TouchableOpacity>
                            </View>
                        );
                      }) : <Text>Please Wait</Text>}
                </View>
                <TouchableOpacity style={styles.button} onPress={ValidateFields}>
                  <Text style={styles.text}>Zarejestruj się</Text>
                </TouchableOpacity>
              </View>
            </View>
          </View>
          <View>
            <TouchableOpacity>
              <Text style={styles.register} onPress={GoToLogin}>
                Masz już konto? Zaloguj się.
              </Text>
            </TouchableOpacity>
          </View>
        </View>
      </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: 'white',
  },
  selected: {
    margin: 5,
    borderWidth: 2,
    borderColor: COLORS.second,
    borderRadius: 20,
  },
  section: {
    justifyContent: 'center',
    display: 'flex',
    flexDirection: 'column',
    flexWrap: 'wrap',
    alignItems: 'center',
    alignContent: 'center',
    backgroundColor: COLORS.second,
  },
  box: {
    justifyContent: 'center',
    display: 'flex',
    flexDirection: 'column',
    flexWrap: 'wrap',
    alignItems: 'center',
    alignContent: 'center',
    backgroundColor: 'white',
    width: '100%',
    borderTopLeftRadius: 40,
    borderTopRightRadius: 40,
    padding: 20,
  },
  row: {
    justifyContent: 'center',
    flexDirection: 'row',
    flexWrap: 'wrap',
    borderRadius: 20,
    borderColor: COLORS.second,
    borderWidth: 5,
  },
  column: {
    justifyContent: 'center',
    display: 'flex',
    flexDirection: 'column',
    flexWrap: 'wrap',
    alignItems: 'center',
    alignContent: 'center',
  },
  button: {
    justifyContent: 'center',
    borderBottomWidth: 1,
    padding: 10,
    marginTop: 20,
    width: 250,
    borderRadius: 5,
    borderColor: COLORS.thirdOrange,
    backgroundColor: COLORS.second,
  },
  buttonBack: {
    justifyContent: 'center',
    borderBottomWidth: 1,
    padding: 10,
    width: 250,
    borderRadius: 5,
    borderColor: COLORS.thirdOrange,
    backgroundColor: COLORS.main,
  },
  text: {
    textAlign: 'center',
    color: 'white',
  },
  textBack: {
    textAlign: 'center',
    color: COLORS.second,
  },
  textWelcome: {
    textAlign: 'center',
    color: COLORS.main,
    fontSize: 24,
    fontWeight: 'bold',
    margin: 20,
  },
  textWelcome2: {
    textAlign: 'center',
    color: COLORS.second,
    fontSize: 12,
    fontWeight: 'bold',
  },
  textInput: {
    fontSize: 16,
    textAlign: 'left',
    color: COLORS.second,
    marginTop: 20,
    width: 250,
  },
  input: {
    padding: 5,
    paddingLeft: 15,
    marginBottom: 10,
    borderBottomWidth: 2,
    borderRadius: 5,
    borderColor: COLORS.second,
    textAlign: 'left',
    color: COLORS.second,
  },
  smallInput: {
    padding: 5,
    marginBottom: 10,
    borderBottomWidth: 2,
    borderRadius: 5,
    borderColor: COLORS.second,
    textAlign: 'center',
    color: COLORS.second,
    width: 50,
  },
  logo: {
    justifyContent: 'center',
    alignItems: 'center',
    alignContent: 'center',
    width: 200,
    height: 150,
    resizeMode: 'contain',
  },
  logoContainer: {
    margin: 10,
    padding: 10,
  },
  register: {
    textAlign: 'center',
    marginTop: -10,
    marginBottom: 20,
  },
  wrongMail: {
    color: 'red',
  },
});
