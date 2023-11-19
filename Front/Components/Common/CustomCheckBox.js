import React, {useEffect, useState} from "react";
import {useRoute} from "@react-navigation/native";
import axios from "axios";
import store from "../store";
import {API_URL, NOINTERNET, SERVER_ERROR} from "../actions";
import {getUserName, LogOut} from "../Utilities";
import {showMessage} from "react-native-flash-message";
import {COLORS} from "../Colors";
import NetInfo from "@react-native-community/netinfo";
import {Image, ScrollView, StyleSheet, Text, TextInput, TouchableOpacity, View} from "react-native";
import Gallery from "react-native-image-gallery";
import Icon from "react-native-vector-icons/Fontisto";
import {Input} from "react-native-elements";

export default function CustomCheckBox({onChecked,onUnchecked ,style,title,initialState=false}) {
    const [Pressed, setPressed] = useState(initialState);
    return (

        <TouchableOpacity  onPress={() => {
            setPressed(!Pressed);
            if(!Pressed) {
                onChecked()
            }else {
                onUnchecked()
            }
        }}>
            {!Pressed ? <Icon style={style} name={"checkbox-passive"}></Icon> : <Icon style={style} name={"checkbox-active"}></Icon>}
        </TouchableOpacity>


    );
}
const styles = StyleSheet.create({

    iconOn: {
        margin: 0,
        fontSize: 30,
        color: COLORS.second,
    },

    iconOff: {
        borderColor: 'black',
        margin: 0,
        fontSize: 30,
        color: "grey",
    },
    row: {
        borderRadius: 10,
        justifyContent: 'flex-end',
        display: 'flex',
        flexDirection: 'row',
        alignContent: 'flex-start',
        marginTop: 20,
        margin: 10,
        padding: 20,
    }
});
