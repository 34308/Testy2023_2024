import React, { useEffect, useState } from "react";
import { API_URL, NOINTERNET, SERVER_ERROR } from "../../actions";
import { getUserName, LogOut } from "../../Utilities";
import { COLORS } from "../../Colors";
import { Image, ScrollView, StyleSheet, Text, TextInput, TouchableOpacity, View } from "react-native";
import Icon from "react-native-vector-icons/FontAwesome";

interface PressableStarProps {
    onChangeScore: (score: number) => void;
}

export default function PressableStar(props: PressableStarProps) {
    const [Pressed, setPressed] = useState<boolean>(false);
    const [Pressed2, setPressed2] = useState<boolean>(false);
    const [Pressed3, setPressed3] = useState<boolean>(false);
    const [Pressed4, setPressed4] = useState<boolean>(false);
    const [Pressed5, setPressed5] = useState<boolean>(false);

    function GetStar(): number {
        if (Pressed5) {
            return 5;
        } else if (Pressed4) {
            return 4;
        } else if (Pressed3) {
            return 3;
        } else if (Pressed2) {
            return 2;
        } else if (Pressed) {
            return 1;
        } else {
            return 0;
        }
    }

    function PressedStar(i: number): void {
        switch (i) {
            case 1:
                setPressed(!Pressed);
                if (Pressed4) {
                    props.onChangeScore(0);
                } else {
                    props.onChangeScore(1);
                }
                break;
            case 2:
                setPressed(!Pressed2);
                setPressed2(!Pressed2);
                setPressed3(false);
                setPressed4(false);
                setPressed5(false);
                if (Pressed4) {
                    props.onChangeScore(0);
                } else {
                    props.onChangeScore(2);
                }
                if (Pressed4) {
                    props.onChangeScore(0);
                } else {
                    props.onChangeScore(2);
                }
                break;
            case 3:
                setPressed(!Pressed3);
                setPressed2(!Pressed3);
                setPressed3(!Pressed3);
                setPressed4(false);
                setPressed5(false);
                if (Pressed3) {
                    props.onChangeScore(0);
                } else {
                    props.onChangeScore(3);
                }
                break;
            case 4:
                setPressed(!Pressed4);
                setPressed2(!Pressed4);
                setPressed3(!Pressed4);
                setPressed4(!Pressed4);
                setPressed5(false);
                if (Pressed4) {
                    props.onChangeScore(0);
                } else {
                    props.onChangeScore(4);
                }
                break;
            case 5:
                setPressed(!Pressed5);
                setPressed2(!Pressed5);
                setPressed3(!Pressed5);
                setPressed4(!Pressed5);
                setPressed5(!Pressed5);
                if (Pressed5) {
                    props.onChangeScore(0);
                } else {
                    props.onChangeScore(5);
                }

                break;
            default:
                props.onChangeScore(0);
                break;
        }
    }

    return (
        <View style={styles.row}>
            <TouchableOpacity onPress={() => { PressedStar(1) }}>
                {Pressed ? <Icon name="star" style={styles.iconOn} /> : <Icon name="star" style={styles.iconOff} />}
            </TouchableOpacity>
            <TouchableOpacity onPress={() => { PressedStar(2) }}>
                {Pressed2 ? <Icon name="star" style={styles.iconOn} /> : <Icon name="star" style={styles.iconOff} />}
            </TouchableOpacity>
            <TouchableOpacity onPress={() => { PressedStar(3) }}>
                {Pressed3 ? <Icon name="star" style={styles.iconOn} /> : <Icon name="star" style={styles.iconOff} />}
            </TouchableOpacity>
            <TouchableOpacity onPress={() => { PressedStar(4) }}>
                {Pressed4 ? <Icon name="star" style={styles.iconOn} /> : <Icon name="star" style={styles.iconOff} />}
            </TouchableOpacity>
            <TouchableOpacity onPress={() => { PressedStar(5) }}>
                {Pressed5 ? <Icon name="star" style={styles.iconOn} /> : <Icon name="star" style={styles.iconOff} />}
            </TouchableOpacity>
        </View>
    );
}

const styles = StyleSheet.create({
    iconOn: {
        margin: 0,
        fontSize: 30,
        color: COLORS.fourth,
    },
    iconOff: {
        borderColor: 'black',
        margin: 0,
        fontSize: 30,
        color: COLORS.second,
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
