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
import Icon from "react-native-vector-icons/FontAwesome";
import {Input} from "react-native-elements";
import PressableStar from "./CommentsComponents/PressableStar";
import {wrap} from "@babel/runtime/regenerator";
import jwtDecode from "jwt-decode";

export default function Searchbar(props) {
    const [phrase, setPhrase] = useState('');

    return (
        <View style={styles.searchBar}>
            <Icon name="search" style={styles.icon} />
            <TextInput
                value={phrase}
                onChangeText={(value)=>{setPhrase(value); props.OnSearch(value)}}
                placeholder="Some place"
                style={styles.textSearchBar}
            />
        </View>
    );
}
const styles = StyleSheet.create({
    container: {
        justifyContent: 'center',
        display: 'flex',
        flexDirection: 'row',
        flexWrap: 'wrap',
        alignItems: 'center',
        alignContent: 'center',
        backgroundColor: 'white',
        marginTop: 10,

    },
    dishContainer: {
        justifyContent: 'center',
        display: 'flex',
        flexDirection: 'row',
        flexWrap: 'wrap',
        alignItems: 'center',
        alignContent: 'center',
        backgroundColor: 'white',
        margin: 0,
    },
    textSearchBar: {
        color: COLORS.second,
        fontFamily: 'Ubuntu-Light',
        fontSize: 16,
        textAlign: 'left',
    },
    textTitle: {
        fontSize: 16,
        color: COLORS.second,
        marginBottom: 10,
    },
    textDesc: {
        fontSize: 12,
        color: 'black',
    },
    textPrice: {
        fontSize: 16,
        color: COLORS.second,
        fontWeight: 800,
        marginBottom: 10,
    },
    column: {
        width: 190,
        marginRight: 10,
    },
    columnPrice: {
        justifyContent: 'center',
        display: 'flex',
        flexDirection: 'column',
        flexWrap: 'wrap',
        alignItems: 'center',
    },
    row: {
        flex: 1,
        flexDirection: 'row',
        marginTop: 10,
        padding: 5,
        paddingBottom: 15,
        borderBottomWidth: 1,
        borderColor: COLORS.second,
    },
    plusContainer: {
        justifyContent: 'center',
        display: 'flex',
        flexDirection: 'row',
        flexWrap: 'wrap',
        alignItems: 'center',
        alignContent: 'center',
        width: 35,
        borderRadius: 5,
        backgroundColor: COLORS.second,
        marginTop: 20,
    },
    searchBar: {
        justifyContent: 'flex-start',
        display: 'flex',
        flexDirection: 'row',
        flexWrap: 'wrap',
        alignItems: 'center',
        alignContent: 'center',
        backgroundColor: 'transparent',
        borderColor: COLORS.second,
        borderWidth: 1,
        borderRadius: 5,
        width: 380,
        height: 50,
        marginLeft: 15,
    },
    icon: {
        fontSize: 18,
        marginLeft: 10,
        marginRight: 5,
        color: COLORS.second,
    },
    iconPlus: {
        fontSize: 26,
        padding: 5,
        color: 'white',
    },

});
