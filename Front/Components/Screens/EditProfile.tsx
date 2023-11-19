import React, { FC, useEffect, useState } from 'react';
import {
  Button,
  Image,
  StyleSheet,
  Text,
  TextInput,
  TouchableOpacity,
  View,
  ScrollView, Modal,
} from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { COLORS } from '../Colors';
import ModalSpotsLists from "../Common/ModalSpotsLists";
import ModalEditPassword from "../Common/AdditionalSettingsComponents/ModalEditPassword";
import EditLoginEmail from "../Common/AdditionalSettingsComponents/EditLogin&Email";


interface EditProfileProps {
  navigation: any;
}

const EditProfile: FC<EditProfileProps> = ({ navigation }) => {
  const [emailModalVisible, setEmailModalVisible] = useState<boolean>(false);
  const [passwordModalVisible, setPasswordModalVisible] = useState<boolean>(false);
  const [loginModalVisible, setLoginModalVisible] = useState<boolean>(false);

  useEffect(() => {
    const unsubscribe = navigation.addListener('focus', () => {});

    return unsubscribe;
  });

  function GoToSetings() {
    navigation.goBack();
  }
  return (
      <ScrollView>
        <View style={{display:"flex",justifyContent:"center",alignSelf:"center"}}>
          <TouchableOpacity style={styles.button} onPress={()=>{setPasswordModalVisible(!passwordModalVisible)}}>
            <Text style={styles.buttonText}>Zmień hasło</Text>
          </TouchableOpacity>
          {passwordModalVisible?<ModalEditPassword OnClose={()=>{ setPasswordModalVisible(false)}}/>:null}
          <TouchableOpacity style={styles.button} onPress={()=>{setLoginModalVisible(!loginModalVisible)}}>
            <Text style={styles.buttonText}>Zmień nazwe konta</Text>
          </TouchableOpacity>
          {loginModalVisible?<EditLoginEmail ChangeLogin={true} OnClose={()=>{setLoginModalVisible(false)}}/> : null }
          <TouchableOpacity style={styles.button} onPress={()=>{setEmailModalVisible(!emailModalVisible)}}>
            <Text style={styles.buttonText}>Zmień email</Text>
          </TouchableOpacity>
          {emailModalVisible?<EditLoginEmail ChangeLogin={false} OnClose={()=>{ setEmailModalVisible(false)}}/> : null }

        </View>

      </ScrollView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: 'white',
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
    flexDirection: 'row',
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
    fontWeight: '800',
    margin: 20,
  },
  textWelcome2: {
    textAlign: 'center',
    color: COLORS.second,
    fontSize: 12,
    fontWeight: '800',
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
  }, buttonText: {
    color:COLORS.main,
    fontSize:18,
    justifyContent:"center",
    alignSelf:"center",
  }

});

export default EditProfile;
