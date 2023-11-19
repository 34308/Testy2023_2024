import React, {useEffect, useImperativeHandle, useState} from "react";
import axios from "axios";
import store from "../../store";
import {API_URL, COMMENT, NOINTERNET, SERVER_ERROR} from "../../actions";
import {COLORS} from "../../Colors";
import {Image, ScrollView, StyleSheet, Text, TextInput, TouchableOpacity, View} from "react-native";
import PressableStar from "./PressableStar";
import jwtDecode from "jwt-decode";
import {Colors} from "react-native/Libraries/NewAppScreen";
import {showMessage} from "react-native-flash-message";

interface CommentFillableProps {
    touristSpottId: number;
    CloseModal: (value: boolean) => void;
    ParentCommentId?: number;
}

interface JwtDecodedResult {
    jti: string,
    sub: string,
    role: string,
    // Add other properties if jwtDecode returns more than just 'jti'
}

export default function CommentForCommentFillable(props: CommentFillableProps) {
    const url = API_URL + COMMENT + 'comment/';
    const [Value, setValue] = useState<number>(0);
    const [title, setTitle] = useState<string>('');
    const [description, setDesc] = useState<string>('');
    const [Token, setToken] = useState<JwtDecodedResult>(jwtDecode(store.getState().token));
    const [AvatarIn,setAvatarIn]=useState(false);
    const [Avatar,setAvatar]=useState(store.getState().Avatar);


    async function setAv() {
        if (store.getState().Avatar) {
            setAvatar(await store.getState().Avatar);
            setAvatarIn(true)
        }
    }
    function CallBack(data: number) {
        setValue(data)
    }
    useEffect(() => {
        setAv()
        if (!store.getState().isLoggedIn || store.getState().Avatar==undefined) {
            showMessage({
                message: 'Nie jesteś zalogowany',
                type: 'info',
                backgroundColor: COLORS.mainOrange,
                color: 'black',
            });
            props.CloseModal(false);
        }
    }, []);

    function addComment() {
        const decodedToken = jwtDecode(store.getState().token) as JwtDecodedResult;
        setToken(decodedToken);

        const body = {
            "id": "" + 0,
            "title": "" + title,
            "description": "" + description,
            "score": "" + 0,
            "userId": "" + decodedToken.jti,
            "touristSpotId": "" + props.touristSpottId,
            "parentCommentId": "" + props.ParentCommentId == undefined ? 0 : props.ParentCommentId
        }
        axios.post(url + Token.jti, body, {
            headers: {
                'Authorization': store.getState().token,
            }
        })
            .then(response => {
                if (response.data.Status == 0) {
                    showMessage({
                        message: 'Dodano komentarz.',
                        type: 'info',
                        backgroundColor: COLORS.second,
                        color: COLORS.main,
                    });
                    props.CloseModal(false);
                }else{
                    showMessage({
                        message: 'Błąd przy dodawaniu komentarza: '+response.data.Message,
                        type: 'danger',
                        backgroundColor: COLORS.second,
                        color: COLORS.main,
                    });
                }
            })
            .catch(error => {
                console.log('error:' + error.data)
            });
    }

    // @ts-ignore
    return (
        <View style={styles.modalView}>
            <View style={{backgroundColor: 'white'}}>
                <View>
                    <View style={styles.row}>
                        <View style={styles.row2}>
                            <View style={styles.partRow1}>
                                {AvatarIn?< Image style={{
                                    height: 40,
                                    width: 40,
                                    marginBottom: 10,
                                }} source={{uri: Avatar}}></Image>:null}

                                <Text style={styles.textDesc}>{Token.sub}</Text>
                            </View>
                        </View>
                        <View>
                            <TextInput style={styles.Title} onChangeText={(v) => setTitle(v)}
                                       placeholder='Title'></TextInput>
                            <TextInput style={styles.container} onChangeText={(v) => setDesc(v)} multiline={true}
                                       placeholder='Description'></TextInput>
                        </View>
                        <View style={styles.row2}>
                            <TouchableOpacity onPress={() => {
                                props.CloseModal(false);
                            }} style={styles.buttonCancel}>
                                <Text style={styles.buttonText}>Cancel</Text>
                            </TouchableOpacity>
                            <TouchableOpacity onPress={() => addComment()} style={styles.button}>
                                <Text style={styles.buttonText}>Comment</Text>
                            </TouchableOpacity>

                        </View>
                    </View>
                </View>

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
    Title: {
        backgroundColor: '#f0f0f0', // Light grey background color
        borderRadius: 10, // Rounded corners
        padding: 10,
        marginBottom: 15,
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
    column: {
        justifyContent: 'center',
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        alignContent: 'center',
    },
    row: {
        borderWidth: 1,
        borderColor: '#ddd',
        borderRadius: 10,
        justifyContent: 'flex-start',
        display: 'flex',
        flexDirection: 'column',
        alignContent: 'center',
        marginTop: 20,
        marginBottom: 10,
        padding: 20,
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
        margin: 0,
        fontSize: 30,
        color: COLORS.second,

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

    elevation: {
        shadowColor: '#ff0000',
        elevation: 20,
    },
    image: {
        width: '100%',

        borderRadius: 5,
        borderColor: COLORS.main,
        borderWidth: 1,
    },
    button: {
        textAlign: 'center',

        justifyContent: 'center',
        marginTop: 40,
        width: 140,
        height: 40,
        borderRadius: 20,
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
