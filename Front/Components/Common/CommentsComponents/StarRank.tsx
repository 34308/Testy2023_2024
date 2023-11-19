import React, { useEffect, useState } from "react";
import store from "../../store";
import { API_URL, NOINTERNET, SERVER_ERROR } from "../../actions";
import { showMessage } from "react-native-flash-message";
import { COLORS } from "../../Colors";
import { Image, ScrollView, StyleSheet, Text, TextInput, TouchableOpacity, View } from "react-native";
import Icon from "react-native-vector-icons/FontAwesome";

interface StarRank{
    commentUrl: string;
    onChangeScore: (score: number) => void;
}
interface props {
    score:number;
}
export default function StarRank(props:props) {
    const [Pressed, setPressed] = useState<boolean>(false);
    const [Pressed2, setPressed2] = useState<boolean>(false);
    const [Pressed3, setPressed3] = useState<boolean>(false);
    const [Pressed4, setPressed4] = useState<boolean>(false);
    const [Pressed5, setPressed5] = useState<boolean>(false);
    useEffect(() => {
      switch (props.score) {
          case 1:
              setPressed(!Pressed)
              break;
          case 2:
              setPressed(!Pressed2)
              setPressed2(!Pressed2)
              setPressed3(false)
              setPressed4(false)
              setPressed5(false)

              break;
          case 3:
              setPressed(!Pressed3)
              setPressed2(!Pressed3)
              setPressed3(!Pressed3)
              setPressed4(false)
              setPressed5(false)

              break;
          case 4:
              setPressed(!Pressed4)
              setPressed2(!Pressed4)
              setPressed3(!Pressed4)
              setPressed4(!Pressed4)
              setPressed5(false)

              break;
          case 5:
              setPressed(!Pressed5)
              setPressed2(!Pressed5)
              setPressed3(!Pressed5)
              setPressed4(!Pressed5)
              setPressed5(!Pressed5)

      }

    }, []); // The empty dependency array means this effect runs once after mounting

    return (
        <View style={styles.row}>
        <View>
        {Pressed ? <Icon name="star" style={styles.iconOn} /> : <Icon name="star" style={styles.iconOff} />}
        </View>
        <View >
        {Pressed2 ? <Icon name="star" style={styles.iconOn} /> : <Icon name="star" style={styles.iconOff} />}
        </View>
            <View>
        {Pressed3 ? <Icon name="star" style={styles.iconOn} /> : <Icon name="star" style={styles.iconOff} />}
        </View>
            <View>
        {Pressed4 ? <Icon name="star" style={styles.iconOn} /> : <Icon name="star" style={styles.iconOff} />}
        </View>
    <View>
        {Pressed5 ? <Icon name="star" style={styles.iconOn} /> : <Icon name="star" style={styles.iconOff} />}
        </View>
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
        color: COLORS.third,
    },
    row: {
        borderRadius: 10,
        justifyContent: 'flex-end',
        display: 'flex',
        flexDirection: 'row',
        alignContent: 'flex-start',
        marginTop: 20,

    }
});
