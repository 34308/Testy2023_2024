import React, {useEffect, useState} from "react";
import {useRoute} from "@react-navigation/native";
import axios from "axios";
import store from "../store";
import {API_URL, NOINTERNET, SERVER_ERROR, SETPINS, TOURIST_SPOT} from "../actions";
import {getUserName, LogOut} from "../Utilities";
import {showMessage} from "react-native-flash-message";
import {COLORS} from "../Colors";
import NetInfo from "@react-native-community/netinfo";
import {Dimensions, FlatList, Image, ScrollView, StyleSheet, Text, TouchableOpacity, View} from "react-native";
import Gallery from "react-native-image-gallery";
import Icon from "react-native-vector-icons/FontAwesome";
import {GestureHandlerRootView, Swipeable} from "react-native-gesture-handler";
import {useDispatch} from "react-redux";
import {SafeAreaView} from "react-native-safe-area-context";

const dimensions = Dimensions.get('window');
const imageHeight = Math.round((dimensions.width * 9) / 16);
const imageWidth = dimensions.width;
export default function PinsLocationList(props) {
    const [Pins, setPins] = useState([]);
    const [TouristSpotPin, SetTouristSpotPins] = useState([]);
    const dispatch = useDispatch();
    const url = API_URL + TOURIST_SPOT + 'getSpotsForPins';

    function extractIdsFromList(places) {
        return places.map(place => place.id);
    }

    useEffect(() => {
        const PinsIds = extractIdsFromList(props.pins)
        setPins(props.pins);
        const fetchData = async () => {
            try {
                axios.post(url, {ids: PinsIds}).then((result) => {
                    SetTouristSpotPins(result.data.Data)
                });
            } catch (error) {
                console.log('error', error);
            }
        };
        fetchData();

    }, [url]);

    async function deletePin(place) {
        if (Pins.length > 0) {
            const idExists = Pins.some(item => item.id === place.Id);
            if (!idExists) {
                showMessage({
                    message: 'Nie znaleziono w nawigacji',
                    type: 'info',
                    backgroundColor: COLORS.mainOrange,
                    color: 'black',
                });
            } else {
                const newPins = Pins.filter((item, i) => {
                    return item.id !== place.Id;
                });
                console.log('pins1')
                dispatch({type: SETPINS, payload: JSON.stringify(newPins)});
                showMessage({
                    message: 'Usunieto z nawigajci',
                    type: 'info',
                    backgroundColor: COLORS.mainOrange,
                    color: 'black',
                });
            }
        } else {
            showMessage({
                message: 'Nawigacja jest pusta.',
                type: 'info',
                backgroundColor: COLORS.mainOrange,
                color: 'black',
            });
        }
        setPins(JSON.parse(await store.getState().Pin));
    }

    const LeftSwipeActions = () => {
        return (
            <View
                style={{flex: 1, backgroundColor: '#ccffbd', justifyContent: 'center'}}
            >
                <Text
                    style={{
                        color: '#40394a',
                        paddingHorizontal: 10,
                        fontWeight: '600',
                        paddingVertical: 20,
                    }}
                >
                    Bookmark
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
                }}
            >
                <Text
                    style={{
                        color: '#1b1a17',
                        paddingHorizontal: 10,
                        fontWeight: '600',
                        paddingVertical: 20,
                    }}
                >
                    Usuń
                </Text>
            </View>
        );
    };
    const swipeFromLeftOpen = () => {
        alert('Swipe from left');
    };

    function deleteFromTouristSpot(place) {
        const idExists = TouristSpotPin.some(item => item.Id === place.Id);
        if (idExists) {
            const newTouristSpotPins = TouristSpotPin.filter((item, i) => {
                return item.Id !== place.Id;
            });

            SetTouristSpotPins(newTouristSpotPins)
        }
    }

    const swipeFromRightOpen = (item) => {

        deletePin(item)
        deleteFromTouristSpot(item)
        props.OnDeletePin();
    };

    function SwipapbleTouristSpot(item) {

        return (
            <Swipeable
                renderLeftActions={LeftSwipeActions}
                renderRightActions={rightSwipeActions}
                onSwipeableRightOpen={() => swipeFromRightOpen(item)}
                onSwipeableLeftOpen={() => swipeFromLeftOpen()}
            >

                <View style={styles.row} key={item.Id}>
                    <Image
                        style={styles.image}
                        source={{uri: item.Photo.Photo}}
                    />
                    <Text style={styles.textLogin}>{item.Name}</Text>
                </View>
            </Swipeable>
        );
    }

    return (
        <GestureHandlerRootView style={styles.modalView}>
            <SafeAreaView>
                <View style={styles.main}>
                    <FlatList data={TouristSpotPin}
                              renderItem={({item, i}) => <SwipapbleTouristSpot{...item} />}
                    >
                    </FlatList>
                </View>
            </SafeAreaView>
        </GestureHandlerRootView>
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
