import React, {useEffect, useImperativeHandle, useState} from "react";
import axios from "axios";
import Icon from 'react-native-vector-icons/Fontisto';
import store from "../../store";
import {API_URL, COMMENT, NOINTERNET, SERVER_ERROR, USER} from "../../actions";
import {COLORS} from "../../Colors";
import {Alert, Image, ScrollView, StyleSheet, Text, TextInput, TouchableOpacity, View} from "react-native";
import PressableStar from "../CommentsComponents/PressableStar";
import jwtDecode from "jwt-decode";
import {Colors} from "react-native/Libraries/NewAppScreen";
import {showMessage} from "react-native-flash-message";
import {JwtDecodedResult} from "../Interfaces";
import {AxiosResponse} from "axios/index";

interface modalEditPasswordProps {
    OnClose: () => void;

}

export default function ModalEditPassword(props: modalEditPasswordProps) {
    const [Token, setToken] = useState<JwtDecodedResult>(jwtDecode(store.getState().token));
    const [NewPassword, setNewPassword] = useState<string>("")
    const [NewPasswordRepeat, setNewPasswordRepeat] = useState<string>("")
    const [OldPassword, setOldPassword] = useState<string>("")

    const [STENewPassword, setSTENewPassword] = useState<boolean>(false)
    const [STENewPasswordRepeat, setSTENewPasswordRepeat] = useState<boolean>(false)
    const [STEOldPassword, setSTEOldPassword] = useState<boolean>(false)

    useEffect(() => {
        if(!store.getState().isLoggedIn ){
            showMessage({
                message: 'Nie jesteś zalogowany',
                type: 'info',
                backgroundColor: COLORS.mainOrange,
                color: 'black',
            });
            props.OnClose();
        }
    }, []);
    function ChangePassword() {
        if (!store.getState().isLoggedIn) {
            showMessage({
                message: 'Nie jesteś zalogowany',
                type: 'info',
                backgroundColor: COLORS.mainOrange,
                color: 'black',
            });
            return;
        }
        try {
            axios.post(API_URL + USER + "updatePassword/" + Token?.jti, {newPassword: NewPassword, oldPassword: OldPassword},
                {headers: {Authorization: store.getState().token}}).then(result => {
                if (result.data.Status == 0) {
                    props.OnClose();
                    showMessage({
                        message: 'Zmieniono poprawnie ',
                        type: 'info',
                        backgroundColor: COLORS.second,
                        color: COLORS.main,
                    });
                } else {
                    showMessage({
                        message: 'Wystąpił błąd ' + result.data.Message,
                        type: 'danger',
                        backgroundColor: COLORS.second,
                        color: COLORS.main,
                    });
                }
            })
        } catch (e) {
            showMessage({
                message: 'Wystąpił błąd. ' + e,
                type: 'danger',
                backgroundColor: COLORS.second,
                color: COLORS.main,
            });
        }
    }

    // @ts-ignore
    return (
        <View
            style={{borderWidth: 2, borderRadius: 20, borderColor: COLORS.second, padding: 20, marginTop: 20, display: "flex"}}>
            <View>
                <View style={styles.row}>
                    <TextInput secureTextEntry={STENewPassword} placeholder={"Nowe hasło"} style={styles.inputField}
                               value={NewPassword} onChangeText={setNewPassword}></TextInput>
                    <TouchableOpacity onPress={() => {
                        setSTENewPassword(!STENewPassword)
                    }}><Icon style={styles.icon} name={"eye"}></Icon></TouchableOpacity>
                </View>
                <View style={styles.row}>
                    <TextInput secureTextEntry={STENewPasswordRepeat} placeholder={"Powtórz hasło"}
                               style={styles.inputField} value={NewPasswordRepeat}
                               onChangeText={setNewPasswordRepeat}></TextInput>
                    <TouchableOpacity onPress={() => {
                        setSTENewPasswordRepeat(!STENewPasswordRepeat)
                    }}><Icon style={styles.icon} name={"eye"}></Icon></TouchableOpacity>
                </View>
                <View style={styles.row}>
                    <TextInput secureTextEntry={STEOldPassword} placeholder={"Stare hasło"} style={styles.inputField}
                               value={OldPassword} onChangeText={setOldPassword}></TextInput>
                    <TouchableOpacity onPress={() => {
                        setSTEOldPassword(!STEOldPassword)
                    }}><Icon style={styles.icon} name={"eye"}></Icon></TouchableOpacity>
                </View>
            </View>
            <View style={{marginTop: 12}}>
                <TouchableOpacity style={styles.button} onPress={() => {
                    ChangePassword()
                }}>
                    <Text style={styles.buttonText}>Zmień</Text>
                </TouchableOpacity>
                <TouchableOpacity style={styles.button} onPress={() => {
                    props.OnClose();
                }}>
                    <Text style={styles.buttonText}>Wróć</Text>
                </TouchableOpacity>
            </View>
        </View>

    );
}
const styles = StyleSheet.create({
    main: {
        backgroundColor: COLORS.main,
    },
    container: {
        backgroundColor: '#f0f0f0', // Light grey background color
        borderRadius: 10, // Rounded corners
        padding: 10,
        paddingBottom: 60,// Padding to create some space
    },
    inputField: {
        height: 40, // Specify the height of the input field
        margin: 12,
        marginLeft: 0,// Margin from the elements around it
        borderWidth: 1, // Width of the border
        padding: 10, // Padding inside the input field
        borderColor: COLORS.second, // Color of the border
        borderRadius: 5, // Rounded corners
        color: "black", // Text color
        fontSize: 16, // Text size
        backgroundColor: COLORS.main,
    },

    centeredView: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
        marginTop: 22,
    },
    modalView: {
        flex: 1,
        justifyContent: 'center',
        margin: 10,
        backgroundColor: 'transparent',
        padding: 15,
        shadowOffset: {
            width: 0,
            height: 2,
        },
        shadowOpacity: 0.25,
        shadowRadius: 4,
    },

    row: {
        display: 'flex',
        flexDirection: 'row',
        alignItems: "center",
        alignSelf: "center",
        justifyContent: "center"
    },
    commentContent: {
        display: "flex",
        flexDirection: "column",
    },
    row2: {
        width: '100%',
        justifyContent: 'space-between',
        display: 'flex',
        flexDirection: 'row',

    },
    partRow1: {
        justifyContent: 'flex-start',
        display: 'flex',
        flexDirection: 'row',
        alignItems: 'flex-start',
        alignContent: 'flex-start',
        marginTop: 20,
    },
    partRow2: {
        justifySelf: 'flex-end',
        display: 'flex',
        flexDirection: 'row',
        alignItems: 'flex-end',
        alignContent: 'flex-end',
        marginBottom: 10,
        marginTop: -30,
        marginRight: -40,

    },
    counterText: {
        textAlign: 'center',
        fontSize: 22,
        color: COLORS.second,
        fontWeight: 800,
        marginLeft: 20,
        marginRight: 20,
    },
    icon: {
        margin: 12,
        marginTop: 20,
        marginLeft: 0,
        fontSize: 20,

        color: COLORS.second,

        flex: 1,
    },
    textTitle: {
        fontSize: 24,
        color: COLORS.second,
        marginTop: 20,
    },

    textDesc: {
        fontSize: 22,
        color: 'black',
        marginLeft: 20,
        marginTop: 10,
    },
    textDescTitle: {
        fontSize: 20,
        color: COLORS.second,
        marginBottom: 10,
    },
    descContainer: {
        borderBottomWidth: 1,
        borderTopWidth: 1,
        borderColor: COLORS.second,
        borderRadius: 5,
        padding: 10,
        width: 350,
        marginTop: 10,
    },

    button: {
        textAlign: 'center',
        justifyContent: 'center',
        height: 40,
        margin: 20,
        marginTop: 0,
        borderRadius: 20,
        marginBottom: 5,

        backgroundColor: COLORS.second,
    },
    buttonCancel: {

        justifyContent: 'center',
        justifySelf: 'center',
        marginTop: 40,
        width: 140,
        height: 40,
        borderRadius: 20,
        backgroundColor: '#646464'
    },
    buttonText: {
        textAlign: 'center',
        color: COLORS.main,
        fontSize: 18,
        fontWeight: "800"
    },
    buttonIcon: {
        color: COLORS.second,
        fontSize: 20,
    },
    rowButton: {
        justifyContent: 'center',
        display: 'flex',
        flexDirection: 'row',
    },
});
