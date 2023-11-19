import React, {useEffect, useState} from "react";
import {useRoute} from "@react-navigation/native";
import axios from "axios";
import store from "../store";
import {API_URL, DELETEPINS, NOINTERNET, ROUTE, SERVER_ERROR, SETPINS} from "../actions";
import {getUserName, LogOut} from "../Utilities";
import {showMessage} from "react-native-flash-message";
import {COLORS} from "../Colors";
import {Dimensions, FlatList, Image, Modal, ScrollView, StyleSheet, Text, TouchableOpacity, View} from "react-native";
import {GestureHandlerRootView, Swipeable} from "react-native-gesture-handler";
import {useDispatch} from "react-redux";
import {SafeAreaView} from "react-native-safe-area-context";
import jwtDecode from "jwt-decode";
import {Coordinates, JwtDecodedResult} from "./Interfaces";
import SwipableElement from "./SwipableElement";
import DragableListOfSpots from "./DragableListOfSpots";

interface Pin {
    id: number;
}

interface TouristSpotPin {
    Id: number;
    UserId: number;
    RouteName: string;
}

interface RoutesListProps {
    CloseModal: () => void;
    UserLocation: Coordinates
}

const dimensions = Dimensions.get('window');
const imageHeight = Math.round((dimensions.width * 9) / 16);
const imageWidth = dimensions.width;

export default function ModalRoutesList(props: RoutesListProps) {
    const [Routes, SetRoutes] = useState<TouristSpotPin[]>([]);
    const dispatch = useDispatch();
    const [Token, setToken] = useState<JwtDecodedResult>(jwtDecode(store.getState().token));
    const [ModalVisible, setModalVisible] = useState<boolean>(false);
    const [ModalItem, setModalItem] = useState<TouristSpotPin>({Id:0,UserId:0,RouteName:""});

    const url = API_URL + ROUTE + "getUserRoutes/" + Token.jti

    function extractIdsFromList(places: Pin[]): number[] {
        return places.map(place => place.id);
    }

    useEffect(() => {
        const fetchData = async () => {
            try {
                axios.get(url, {headers: {Authorization: store.getState().token}}).then(result => {
                    SetRoutes(result.data.Data);

                });
            } catch (error) {
                console.log('error', error);
            }
        };

        fetchData();
    }, [url]);

    async function deleteRoute(place: TouristSpotPin) {
    }

    function SetRouteForMapSegregated(item: TouristSpotPin) {
        dispatch({type: DELETEPINS});
        try {
            axios.post(API_URL + ROUTE + "GetRoutePinsForRoute/true", {
                "userLocation": {
                    "latitude": props.UserLocation.latitude,
                    "longitude": props.UserLocation.longitude,
                    "longitudeDelta": 0.1,
                    "latitudeDelta": 0.1
                },
                "route": {
                    "id": item.Id,
                    "userId": item.UserId,
                    "routeName": "" + item.RouteName
                }
            }, {headers: {Authorization: store.getState().token}}).then(result => {
                dispatch({type: SETPINS, payload: JSON.stringify(result.data.Data)});
            });
        } catch (error) {
            console.log('error', error);
        }
    }

    function SetRouteForMap(item: any) {
        dispatch({type: DELETEPINS});
        try {
            axios.post(API_URL + ROUTE + "GetRoutePinsForRoute/false", {
                "userLocation": {
                    "latitude": props.UserLocation.latitude,
                    "longitude": props.UserLocation.longitude,
                    "longitudeDelta": 0.1,
                    "latitudeDelta": 0.1
                },
                "route": {
                    "id": item.Id,
                    "userId": item.UserId,
                    "routeName": "" + item.RouteName
                }
            }, {headers: {Authorization: store.getState().token}}).then(result => {
                dispatch({type: SETPINS, payload: JSON.stringify(result.data.Data)});
            });
        } catch (error) {
            console.log('error', error);
        }
    }

    return (
        <GestureHandlerRootView style={styles.modalView}>
            <SafeAreaView>
                <View style={styles.main}>
                    {Routes != null ? <FlatList
                        data={Routes}
                        renderItem={({item}) =>
                            <SwipableElement
                                OnClick={() => {
                                    setModalVisible(true)
                                    setModalItem(item)
                                }}
                                LeftMessage={"Dodaj do nawigacji (Posegregowane)"}
                                RightMessage={"Dodaj do nawigacji"}
                                CloseModal={() => props.CloseModal()}
                                SwipeableItem={{ItemName: item.RouteName, Id: item.Id}}
                                SwipeFromLeftAction={() => {
                                    SetRouteForMapSegregated(item)
                                }}
                                SwipeFromRightAction={() => {
                                    SetRouteForMap(item)
                                }}
                            />}
                    ></FlatList> : null}
                </View>
            <Modal

                      animationType="slide"
                      transparent={true}
                      visible={ModalVisible}
                      onRequestClose={() => {
                         setModalVisible(false);
                      }}
            >
                <DragableListOfSpots Route={{RouteName:ModalItem.RouteName,Id:ModalItem.Id}} CloseModal={()=>{setModalVisible(false)}}></DragableListOfSpots>
            </Modal>
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
