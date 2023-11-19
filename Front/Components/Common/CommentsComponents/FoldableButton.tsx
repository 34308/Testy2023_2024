import React, {useEffect, useImperativeHandle, useState} from "react";
import axios from "axios";
import store from "../../store";
import {API_URL, NOINTERNET, SERVER_ERROR} from "../../actions";
import {COLORS} from "../../Colors";
import {Image, ScrollView, StyleSheet, Text, TextInput, TouchableOpacity, View} from "react-native";
import PressableStar from "./PressableStar";
import jwtDecode from "jwt-decode";
import {Colors} from "react-native/Libraries/NewAppScreen";
import {showMessage} from "react-native-flash-message";

interface FoldableButtonsProps {
    OnPress: () => void;
    CommentNumber: number;
}

export default function FoldableButton(props: FoldableButtonsProps) {
    const [folded, setFolded] = useState<boolean>(true);

    function CallBack() {
    }

    // @ts-ignore
    return (
        <View>
            <TouchableOpacity onPress={() => {
                props.OnPress();
                setFolded(!folded)
            }}>
                <Text>{folded ? (props.CommentNumber > 0 ? "Rozwiń : " + props.CommentNumber : "") : "Zwiń"}</Text>
            </TouchableOpacity>
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
