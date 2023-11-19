import React, {FC, useState} from 'react';
import {Alert, Modal, StyleSheet, Text, TouchableOpacity, View} from 'react-native';
import Icon from 'react-native-vector-icons/FontAwesome';
import {getUserName, LogOut} from '../Utilities';
import store from '../store';
import NetInfo from '@react-native-community/netinfo';
import {API_URL, NOINTERNET, SERVER_ERROR} from '../actions';
import CommentFillable from "../Common/CommentsComponents/CommentFillable";
import ChangeAvatar from "../Common/AdditionalSettingsComponents/ChangeAvatar";
import FlashMessage from "react-native-flash-message";
import {COLORS} from "../Colors";

interface SettingsProps {
    navigation: any;
}

const Settings: FC<SettingsProps> = ({ navigation }) => {
    const [modal, setModalVisible] = useState<boolean>(false);

    const deleteAccount = () => {
        Alert.alert('Usuń konto', 'Czy na pewno chcesz usunąć konto?', [
            {text: 'Usuń', onPress: () => deletePost()},
            {
                text: 'Wróć',
                onPress: () => console.log('Cancel Pressed'),
            },
        ]);
    }

    const getUser = () => {
        let user: string = getUserName(store.getState().token);
        Alert.alert(user);
    }

    const deletePost = async () => {
        let user: string = getUserName(store.getState().token);
        let url: string = API_URL + '/' + user + '/user/delete';
        try {
            await fetch(url, {
                method: 'DELETE',
                headers: new Headers({
                    Authorization: 'Bearer ' + store.getState().token,
                    'Content-Type': 'application/x-www-form-urlencoded',
                }),
            }).catch(error => {
                NetInfo.fetch().then(state => {
                    state.isConnected ? Alert.alert(SERVER_ERROR + error) : Alert.alert(NOINTERNET);
                });
            });
            LogOut(navigation, store.dispatch);
            console.log('Delete successful.');
        } catch (error) {
            console.log('Error while deleting account.');
        }
    }

    const OpenChangeAvatarScreen = () => {

    }

    const goToEditScreen = () => {
        navigation.navigate('EditScreen');
    }
    return (
        <View style={styles.container}>
            <View style={styles.box}>
                <TouchableOpacity
                    onPress={() => goToEditScreen()}
                    style={styles.borderBox}>
                    <View style={styles.row}>
                        <Icon name="user" style={styles.iconLeft}/>
                        <Text style={styles.text}>Edytuj profil</Text>
                    </View>
                </TouchableOpacity>
                <TouchableOpacity
                    style={styles.borderBox}
                    onPress={() => {
                        setModalVisible(!modal)
                    }}>
                    <View style={styles.row}>
                        <Icon name="photo" style={styles.iconLeft}/>
                        <Text style={styles.text}>Zmien Avatar</Text>
                    </View>
                </TouchableOpacity>
                <Modal
                    animationType="slide"
                    transparent={false}
                    visible={modal}
                    onRequestClose={() => {
                        setModalVisible(!modal);
                    }}>
                    <ChangeAvatar CloseModal={setModalVisible}></ChangeAvatar>
                </Modal>
                <TouchableOpacity
                    style={styles.borderBox}
                    onPress={() => deleteAccount()}>
                    <View style={styles.row}>
                        <Icon name="remove" style={styles.iconLeft}/>
                        <Text style={styles.text}>Usuń konto</Text>
                    </View>
                </TouchableOpacity>
            </View>
        </View>
    );
}
export default Settings;
const styles = StyleSheet.create({
    container: {
        flex: 1,
        justifyContent: 'flex-start',
        display: 'flex',
        flexDirection: 'column',
        flexWrap: 'wrap',
        alignItems: 'center',
        alignContent: 'center',
        backgroundColor: 'white',
    },
    box: {
        marginTop: 20,
        width: 350,
    },
    text: {
        marginBottom: 30,
        fontSize: 20,
        color: COLORS.second,
    },
    borderBox: {
        marginTop: 20,
        borderBottomWidth: 1,
        borderColor: COLORS.second,
    },
    row: {
        justifyContent: 'flex-start',
        display: 'flex',
        flexDirection: 'row',
        flexWrap: 'wrap',
        alignItems: 'center',
        alignContent: 'center',
    },
    iconLeft: {
        marginLeft: 10,
        marginRight: 10,
        fontSize: 24,
        color: COLORS.second,
    },
    iconRight: {
        marginLeft: 180,
        marginRight: 10,
        fontSize: 24,
        color: COLORS.second,
    },
});

