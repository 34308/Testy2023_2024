import React, {useEffect, useState} from "react";
import {useRoute} from "@react-navigation/native";
import axios from "axios";
import store from "../store";
import {API_URL, DELETEPINS, NOINTERNET, ROUTE, ROUTE_SPOTS, SERVER_ERROR, SETPINS, TOURIST_SPOT} from "../actions";
import {getUserName, LogOut} from "../Utilities";
import {showMessage} from "react-native-flash-message";
import {COLORS} from "../Colors";
import {Dimensions, FlatList, Image, ScrollView, StyleSheet, Text, TouchableOpacity, View} from "react-native";
import {GestureHandlerRootView, Swipeable} from "react-native-gesture-handler";
import {useDispatch} from "react-redux";
import {SafeAreaView} from "react-native-safe-area-context";
import jwtDecode from "jwt-decode";
import {Coordinates, JwtDecodedResult, RouteInt} from "./Interfaces";
import SwipableElement from "./SwipableElement";
import DraggableFlatListProvider from "react-native-draggable-flatlist/lib/typescript/context/draggableFlatListContext";
import DraggableFlatList, {
    NestableDraggableFlatList,
    NestableScrollContainer,
    RenderItemParams,
    ScaleDecorator,
} from "react-native-draggable-flatlist";
import Icon from "react-native-vector-icons/FontAwesome5";

interface Pin {
    id: number;
}

interface RouteSpots {
    RouteId: number;
    TouristSpotId: number;
    Name: string;
    Photo: string;
    Order: number;
}

interface SendingObject {
    id: number;
    routeId: number;
    touristSpotId: number;
    order: number;
}

interface RoutesListProps {
    CloseModal: () => void;
    Route: RouteInt;
}

const dimensions = Dimensions.get('window');
const imageHeight = Math.round((dimensions.width * 9) / 16);
const imageWidth = dimensions.width;

export default function DragableListOfSpots(props: RoutesListProps) {
    const [RouteSpots, SetRouteSpots] = useState<RouteSpots[]>([]);
    const dispatch = useDispatch();
    const [Token, setToken] = useState<JwtDecodedResult>(jwtDecode(store.getState().token));

    const url = API_URL + TOURIST_SPOT + 'getSpotsForPins';

    function extractIdsFromList(places: Pin[]): number[] {
        return places.map(place => place.id);
    }

    useEffect(() => {
        const fetchData = async () => {
            try {
                axios.get(API_URL + ROUTE + "GetSpotsForRoute/" + props.Route.Id + "/" + Token.jti, {headers: {Authorization: store.getState().token}}).then(result => {
                    SetRouteSpots(result.data.Data);
                });
            } catch (error) {
                console.log('error', error);
            }
        };
        fetchData();
    }, [url]);

    function SetRouteForMap(item: RouteSpots) {
        dispatch({type: DELETEPINS});
    }

    function DragableItem({item, drag, isActive}: RenderItemParams<RouteSpots>) {
        return (
            <ScaleDecorator>
                <TouchableOpacity disabled={isActive} onLongPress={() => {
                    console.log("draging");
                    drag()
                }} style={styles.row} key={item.TouristSpotId}>
                    <Image
                        style={styles.image}
                        source={{uri: item.Photo}}
                    />
                    <Text style={styles.textLogin}>{item.Name}</Text>
                    <Icon name="grip-horizontal" style={styles.icon}></Icon>
                </TouchableOpacity>
            </ScaleDecorator>
        );
    }

    function mapRouteSpotsToSendObject(routeSpots: RouteSpots[]): SendingObject[] {
        return routeSpots.map((spot, index) => ({
            id: index + 1, // Assuming you want to generate an ID based on the array index
            routeId: spot.RouteId,
            touristSpotId: spot.TouristSpotId,
            order: index + 1
        }));
    }

    async function ChangeSpotsList() {
        const OrderedRouteSpots = mapRouteSpotsToSendObject(RouteSpots);
        try {
            axios.post(API_URL + ROUTE_SPOTS + "ChangeRouteSpotsOrder", OrderedRouteSpots, {headers: {Authorization: store.getState().token}}).then(result => {
                props.CloseModal();
                if (result.data.Status === 0) {
                } else {
                    console.log('error');
                }
            });
        } catch (error) {
            console.log('error', error);
        }
    }
    return (
        <GestureHandlerRootView style={styles.modalView}>
            <View>
                <View style={{
                    justifyContent: "flex-end",
                    backgroundColor: 'white',
                    borderColor: COLORS.second
                    , borderRadius: 20
                }}>
                    <DraggableFlatList
                        data={RouteSpots}
                        renderItem={DragableItem}
                        keyExtractor={(item) => "" + item.TouristSpotId}
                        onDragEnd={({data}) => SetRouteSpots(data)}
                    />
                    <View style={styles.row}>
                        <TouchableOpacity
                            onPress={() => {
                                props.CloseModal()
                            }}
                            style={{
                                justifyContent: 'center',
                                alignSelf: 'flex-start',
                                flex: 2,
                                height: 40,
                                borderRadius: 20,
                                backgroundColor: COLORS.second,
                            }}>
                            <Text style={styles.buttonText}>Anuluj</Text>
                        </TouchableOpacity>
                        <TouchableOpacity
                            onPress={() => {
                                ChangeSpotsList()
                            }}
                            style={{
                                justifyContent: 'center',
                                alignSelf: 'flex-end',
                                flex: 2,
                                height: 40,
                                borderRadius: 20,
                                backgroundColor: COLORS.second,
                            }}>
                            <Text style={styles.buttonText}>Zaakceptuj</Text>
                        </TouchableOpacity>
                    </View>
                </View>
            </View>
        </GestureHandlerRootView>

    );
}

const styles = StyleSheet.create({

    main: {
        display: "flex",
        justifyContent: 'flex-end',

        backgroundColor: COLORS.main,
    },
    modalView: {
        flex: 1,
        justifyContent: 'flex-end',
        margin: 10,
        backgroundColor: 'transparent',
        borderRadius: 20,
        padding: 15,

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
        flex: 1,
        justifyContent: "flex-end",
        alignSelf: "center",
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
        flex: 7,
        fontSize: 16,
        color: COLORS.sixth,
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
        padding: 20,
        justifyContent: 'center',
        height: 40,
        borderRadius: 20,
        backgroundColor: COLORS.second,
    },
    buttonText: {
        textAlign: 'center',
        color: 'white',
        fontSize: 18,

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
    },
});
