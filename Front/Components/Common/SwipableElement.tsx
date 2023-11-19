import React, {useEffect, useRef, useState} from "react";
import store from "../store";
import {API_URL, ROUTE} from "../actions";
import {COLORS} from "../Colors";
import {Dimensions, StyleSheet, Text, TouchableHighlight, TouchableOpacity, View} from "react-native";
import {Swipeable} from "react-native-gesture-handler";
import {useDispatch} from "react-redux";
import jwtDecode from "jwt-decode";
import {JwtDecodedResult} from "./Interfaces";

interface Pin {
    id: number;
}

interface ItemDetails {
    Id: number;
    ItemName: string;
}

interface SwipableElementProps {
    LeftMessage: string;
    RightMessage: string;
    CloseModal: () => void;
    SwipeFromLeftAction: () => void;
    SwipeFromRightAction: () => void;
    OnClick: () => void;
    SwipeableItem: ItemDetails;
}

const dimensions = Dimensions.get('window');
const imageHeight = Math.round((dimensions.width * 9) / 16);
const imageWidth = dimensions.width;

export default function SwipableElement(props: SwipableElementProps) {
    const dispatch = useDispatch();
    const [Token, setToken] = useState<JwtDecodedResult>(jwtDecode(store.getState().token));
    const url = API_URL + ROUTE + "getUserRoutes/" + Token.jti
    const LeftSwipeActions = () => {
        return (
            <View style={{flex: 1, backgroundColor: '#ccffbd', justifyContent: 'center'}}>
                <Text style={{color: '#40394a', paddingHorizontal: 10, fontWeight: '600', paddingVertical: 20}}>
                    {"" + props.LeftMessage}
                </Text>
            </View>
        );
    };

    const rightSwipeActions = () => {
        return (
            <View
                style={{
                    backgroundColor: '#ff8303',
                    justifyContent: 'center',
                    alignItems: 'flex-end',
                }}>
                <Text
                    style={{
                        color: '#1b1a17',
                        paddingHorizontal: 10,
                        fontWeight: '600',
                        paddingVertical: 20,
                    }}
                >
                    {"" + props.RightMessage}
                </Text>
            </View>
        );
    };
    const swipeFromLeftOpen = (item: ItemDetails) => {
        props.SwipeFromLeftAction();
        props.CloseModal();
    };
    const swipeFromRightOpen = (item: ItemDetails) => {
        props.SwipeFromRightAction();
        props.CloseModal();
    };
    const swipeableRef: any = useRef(null);
    const closeSwipeable = () => {
        if (swipeableRef != null) {
            if (swipeableRef.current != null) {
                swipeableRef.current.close();
            }
        }
    }

    return (
        <Swipeable
            ref={swipeableRef}
            renderLeftActions={LeftSwipeActions}
            renderRightActions={rightSwipeActions}
            onSwipeableRightOpen={() => {
                swipeFromRightOpen(props.SwipeableItem);
                closeSwipeable()
            }}
            onSwipeableLeftOpen={() => {
                swipeFromLeftOpen(props.SwipeableItem);
                closeSwipeable()
            }}>
            <TouchableOpacity onPress={() => props.OnClick()}>
                <View style={styles.row} key={props.SwipeableItem.Id}>
                    <Text style={styles.textLogin}>{props.SwipeableItem.ItemName} </Text>
                </View>
            </TouchableOpacity>

        </Swipeable>
    );
}

const styles = StyleSheet.create({
    main: {
        display: "flex",
        justifyContent: 'flex-end',
        height: 400,
        backgroundColor: COLORS.main,
    },
    modalView: {
        flex: 1,
        width: dimensions.width,
        justifyContent: 'flex-end',
        backgroundColor: 'transparent',
        borderRadius: 5,
        shadowColor: '#000',
        shadowOffset: {
            width: 0,
            height: 2,
        },
        shadowOpacity: 0.25,
        shadowRadius: 4,
        elevation: 5,
    },
    container: {},
    column: {
        justifyContent: 'flex-start',
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
        flexDirection: 'row',
        alignContent: 'flex-end',
        marginTop: 20,
        margin: 10,
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
        marginTop: 20,
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
        marginTop: 20,
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
    textTitle: {
        fontSize: 24,
        color: COLORS.second,
        marginTop: 20,
    },
    textLogin: {
        fontSize: 16,
        color: COLORS.second,
        marginTop: 14,
        marginLeft: 20,
    },
    textDesc: {
        fontSize: 16,
        color: 'black',
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
    textPrice: {
        marginBottom: 20,
        fontWeight: 800,
        textAlign: 'center',
        fontSize: 22,
        color: COLORS.second,
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
        width: 50,
        height: 50,
        borderRadius: 5,
        borderColor: COLORS.main,
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
        color: 'white',
        fontSize: 18,
        marginLeft: 20,
        fontWeight: 800,
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
