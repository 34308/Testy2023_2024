import React, {useEffect, useRef, useState} from "react";
import {useRoute} from "@react-navigation/native";
import axios, {AxiosResponse} from "axios";
import store from "../../store";
import {API_URL, NOINTERNET, SERVER_ERROR, SETAVATAR} from "../../actions";
import {getUserName, LogOut} from "../../Utilities";
import FlashMessage, {showMessage} from "react-native-flash-message";
import {COLORS} from "../../Colors";
import NetInfo from "@react-native-community/netinfo";
import {Image, ScrollView, StyleSheet, Text, TouchableOpacity, View} from "react-native";
import Gallery from "react-native-image-gallery";
import Icon from "react-native-vector-icons/MaterialCommunityIcons";
import jwtDecode from "jwt-decode";
import {useDispatch} from "react-redux";
import {JwtDecodedResult} from "../Interfaces";

interface Avatar {
    Id: number;
    Picture: string;
}

interface ChangeAvatarProps {
    CloseModal: (value: boolean) => void;
}

export default function ChangeAvatar(props: ChangeAvatarProps) {
    const dispatch = useDispatch();

    const url: string = API_URL + 'User/getAllBasicAvatars';
    const [avatars, setAvatars] = useState<Avatar[]>([]);
    const [avatar, setAvatar] = useState<string>('');
    const [newAvatar, setNewAvatar] = useState<string>('');
    const [newAvatarId, setNewAvatarId] = useState<number>(0);
    const [Token, setToken] = useState<JwtDecodedResult>(jwtDecode(store.getState().token));

    useEffect(() => {

        const fetchData = async () => {
            try {
                axios.get(url).then((result: AxiosResponse) => {
                    setAvatars(result.data.Data)
                });
            } catch (error) {
                console.log('error', error);
            }
        };

        async function setAv() {
            if (store.getState().Avatar) {
                setAvatar(await store.getState().Avatar);
            }
        }

        setAv()
        fetchData();

    }, [url]);

    function UpdateAvatar() {
        try {
            if (!store.getState().isLoggedIn) {
                showMessage({
                    message: 'Nie jesteś zalogowany',
                    type: 'info',
                    backgroundColor: COLORS.mainOrange,
                    color: 'black',
                });
                return;
            }
            const updateUrl: string = 'User/updateAvatar/' + Token.jti + '/' + newAvatarId
            axios.post(API_URL + updateUrl, {}, {
                headers: {Authorization: store.getState().token},
            })
                .then((result: AxiosResponse) => {
                    if (result.data.Status === 0) {
                        dispatch({type: SETAVATAR, payload: newAvatar})
                        props.CloseModal(false);
                        showMessage({
                            message: 'Ustawiono nowy avatar',
                            type: 'info',
                            backgroundColor: COLORS.mainOrange,
                            color: 'black',
                        });
                    } else {
                        props.CloseModal(false);
                        showMessage({
                            message: 'Błąd podczas zmiany avatara, spróbuj ponownie.',
                            type: 'info',
                            backgroundColor: COLORS.mainOrange,
                            color: 'black',
                        });
                    }
                }).catch(error => {
                showMessage({
                    message: 'Błąd: ' + error.message,
                    type: 'info',
                    backgroundColor: COLORS.mainOrange,
                    color: 'black',
                });
            })
        } catch (error) {
            showMessage({
                message: 'Błąd: ' + error,
                type: 'info',
                backgroundColor: COLORS.mainOrange,
                color: 'black',
            });
        }
    }

    return (
        <View>
            <Text style={styles.textTitle}>Wybierz swój nowy avatar</Text>

            <View style={styles.row}>
                <View style={styles.partRow1}>
                    <View>
                        <Text style={styles.textTitle}>Obecny</Text>

                        {avatar !== '' ?
                            <Image style={styles.image} source={{uri: avatar}}/> : null}
                    </View>
                </View>
                <View style={styles.partRow2}>
                    <View>
                        <Text style={styles.textTitle}>Obecny</Text>

                        {avatar !== '' ?
                            <Image style={styles.image} source={{uri: newAvatar}}/> : null}
                    </View>

                </View>
            </View>
            <View style={styles.row2}>

                {avatars.map((item: Avatar, i: number) => {
                    return (
                        <View key={i + item.Id}>
                            <TouchableOpacity onPress={() => {
                                setNewAvatar(item.Picture);
                                setNewAvatarId(item.Id)
                            }} style={{margin: 10,}}>
                                <Image style={styles.image} source={{uri: item.Picture}}/>
                            </TouchableOpacity>

                        </View>
                    );
                })}

            </View>

            <View style={styles.rowButton}>
                <TouchableOpacity onPress={() => {
                    props.CloseModal(false);
                }} style={styles.rowButton}>
                    <Icon style={styles.icon} name={'cancel'}>
                    </Icon>
                    <Text style={styles.buttonText}>Wróć</Text>
                </TouchableOpacity>
                <TouchableOpacity onPress={() => {
                    UpdateAvatar()
                }} style={styles.rowButton}>
                    <Icon style={styles.icon} name={'check'}>
                    </Icon>
                    <Text style={styles.buttonText}>Ustaw Nowy</Text>
                </TouchableOpacity>

            </View>

        </View>
    );
}

const styles = StyleSheet.create({
    main: {
        backgroundColor: COLORS.main,
    },
    row: {
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
        marginLeft: 10,
    },
    partRow2: {
        justifySelf: 'flex-end',
        display: 'flex',
        flexDirection: 'row',
        alignItems: 'flex-end',
        alignContent: 'flex-end',
        marginBottom: 10,

        marginRight: 10,

    },
    container: {backgroundColor: COLORS.main},
    column: {
        justifyContent: 'center',
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        alignContent: 'center',
    },

    commentContent: {
        display: "flex",
        flexDirection: "column",
    },
    row2: {
        padding: 20,
        width: '100%',
        justifyContent: 'center',
        display: 'flex',
        flexDirection: 'row',
        marginTop: 20,
        flexWrap: 'wrap',
    },
    buttonRow: {

        padding: 20,
        width: '100%',
        justifyContent: 'center',
        display: 'flex',
        flexDirection: 'row',
        marginTop: 20,
        flexWrap: 'wrap',
    },
    icon: {
        margin: 0,
        fontSize: 30,
        color: COLORS.second,

    },

    textTitle: {
        fontSize: 24,
        color: COLORS.second,
        marginTop: 20,
    },

    imageContainer: {
        justifyContent: 'center',
        display: 'flex',
        flexDirection: 'column',
        flexWrap: 'wrap',
        alignItems: 'center',
        alignContent: 'center',
        width: '100%',

    },
    elevation: {
        shadowColor: '#ff0000',
        elevation: 20,
    },
    image: {
        height: 90,
        width: 90,
        marginBottom: 10,
        borderRadius: 5,
        borderColor: COLORS.second,
        borderWidth: 1,

    },
    button: {
        justifyContent: 'center',
        marginTop: 40,
        width: 250,
        height: 40,
        borderRadius: 20,
        backgroundColor: COLORS.second,
    },
    buttonText: {
        textAlign: 'center',
        color: COLORS.second,
        fontSize: 18,
        marginLeft: 20,
        fontWeight: '800',
    },
    buttonIcon: {
        color: COLORS.second,
        fontSize: 20,
    },
    rowButton: {
        justifyContent: 'center',
        display: 'flex',
        flexDirection: 'row',
        margin: 20,
    },
});
