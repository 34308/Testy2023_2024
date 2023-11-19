import React, {useEffect, useState} from "react";
import axios from "axios";
import store from "../store";
import {API_URL, NOINTERNET, SERVER_ERROR} from "../actions";
import {COLORS} from "../Colors";
import {GestureHandlerRootView} from "react-native-gesture-handler";
import {SafeAreaView} from "react-native-safe-area-context";
import {
    FlatList,
    Image,
    ScrollView,
    StyleSheet,
    Text,
    TextInput,
    TouchableHighlight,
    TouchableOpacity,
    View
} from "react-native";
import PressableStar from "./CommentsComponents/PressableStar";
import jwtDecode from "jwt-decode";
import {Colors} from "react-native/Libraries/NewAppScreen";
import Icon from "react-native-vector-icons/FontAwesome5";
import {showMessage} from "react-native-flash-message";
import {JwtDecodedResult} from "./Interfaces";
import SwipableElement from "./SwipableElement";
import AnimatedScrollView from "react-native-reanimated/lib/types/lib/reanimated2/component/ScrollView";

interface Route {
    Id: number,
    UserId: number
    RouteName: string,
    // Add other properties if jwtDecode returns more than just 'jti'
}

interface SpotsListprops {
    TouristSpotId: number
    TouristSpotName: string
    onModalClose: () => void;
    // Add other properties if jwtDecode returns more than just 'jti'
}

export default function ModalSpotsLists(props: SpotsListprops) {
    const url = API_URL + 'Route/getUserRoutes/';
    const [NameBarVisible, setNameBarVisible] = useState<boolean>(false);
    const [Routes, setRoutes] = useState<Route[]>([]);
    const [RouteName, setRouteName] = useState<string>('');
    const [Token, setToken] = useState<JwtDecodedResult>(jwtDecode(store.getState().token));
    const fetchData = async () => {
        try {
            axios.get(url + Token.jti, {headers: {Authorization: store.getState().token}}).then((result) => {
                console.log(result.data);
                setRoutes(result.data.Data)
            });
        } catch (error) {
            console.log("error", error);
        }
    };
    useEffect(() => {
        fetchData();
    }, [url]);

    function addRoute() {
        try {
            axios.post(API_URL + "Route/AddUserRoute", {
                id: 0,
                userId: Token.jti,
                routeName: RouteName
            }, {headers: {Authorization: store.getState().token}}).then((result) => {
                if (result.data.Status == 0) {
                    showMessage({
                        message: 'Dodano nową trasę',
                        type: 'info',
                        backgroundColor: COLORS.mainOrange,
                        color: 'black',
                    });

                } else {
                    showMessage({
                        message: 'Błąd: ' + result.data.Message,
                        type: 'danger',
                        backgroundColor: COLORS.mainOrange,
                        color: 'black',
                    });
                }
                fetchData();
            });
        } catch (error) {
            console.log("error", error);
        }
    }

    function AddToRoute(item: Route) {
        try {
            axios.post(API_URL + "Route/AddSpotToRoute", {
                id: 0,
                routeId: item.Id,
                touristSpotId: props.TouristSpotId
            }, {headers: {Authorization: store.getState().token}}).then((result) => {
                if (result.data.Status == 0) {
                    showMessage({
                        message: 'Dodano do trasy',
                        type: 'danger',
                        backgroundColor: COLORS.mainOrange,
                        color: 'black',
                    });
                } else {
                    showMessage({
                        message: 'Błąd: ' + result.data.Message,
                        type: 'danger',
                        backgroundColor: COLORS.mainOrange,
                        color: 'black',
                    });
                }
            });
        } catch (error) {
            console.log("error", error);
        }
    }

    function RemoveFromRoute(item: Route) {
        try {
            axios.post(API_URL + "Route/RemoveSpotFromRoute", {
                id: 0,
                routeId: item.Id,
                touristSpotId: props.TouristSpotId
            }, {headers: {Authorization: store.getState().token}}).then((result) => {
                if (result.data.Status == 0) {
                    showMessage({
                        message: 'Usunięto :' + props.TouristSpotName + ' z trasy',
                        type: 'info',
                        backgroundColor: COLORS.mainOrange,
                        color: 'black',
                    });
                } else {
                    showMessage({
                        message: 'Błąd: ' + result.data.Message,
                        type: 'danger',
                        backgroundColor: COLORS.mainOrange,
                        color: 'black',
                    });
                }
            });
        } catch (error) {
            console.log("error", error);
        }
    }

    // @ts-ignore
    return (
        <View style={styles.modalView}>
            <GestureHandlerRootView>
                <SafeAreaView>
                    <View
                        style={{
                            backgroundColor: 'white',
                            borderRadius: 20,
                            borderWidth: 5,
                            borderColor: COLORS.second
                        }}>
                        <TouchableOpacity onPress={() => {
                            props.onModalClose()
                        }}>
                            <Icon style={styles.closeIcon} name={'times'}></Icon>
                        </TouchableOpacity>
                        <View>
                            <TouchableOpacity style={styles.button} onPress={() => setNameBarVisible(true)}>
                                <Text style={styles.buttonText}>Dodaj nową trasę</Text>
                            </TouchableOpacity>
                            {NameBarVisible ?
                                <View style={styles.row2}>
                                    <TextInput style={styles.TextInput} onChangeText={(words) => setRouteName(words)}
                                               placeholder='Nazwa Trasy'></TextInput>
                                    <View style={styles.column}>
                                        <TouchableOpacity onPress={() => {
                                            addRoute()
                                        }}>
                                            <Icon style={styles.icon} name={'check'}></Icon>
                                        </TouchableOpacity>
                                        <TouchableOpacity onPress={() => {
                                            setNameBarVisible(false)
                                        }}>
                                            <Icon style={styles.icon} name={'times'}></Icon>
                                        </TouchableOpacity>
                                    </View>
                                </View>
                                : null}
                            <FlatList
                                style={{
                                    margin: 10,
                                    maxHeight: 300,
                                    borderRadius: 20,
                                    borderColor: COLORS.second,
                                    borderWidth: 2
                                }}
                                data={Routes}
                                renderItem={({item}) => <SwipableElement
                                    OnClick={()=>{}}
                                    LeftMessage={"Dodaj do nawigacji"}
                                    RightMessage={"Usuń z nawigacji"}
                                    CloseModal={() => null}
                                    SwipeableItem={{ItemName: item.RouteName, Id: item.Id}}
                                    SwipeFromLeftAction={() => {
                                        AddToRoute(item)
                                    }}
                                    SwipeFromRightAction={() => {
                                        RemoveFromRoute(item);
                                    }}
                                />}
                            ></FlatList>
                        </View>
                    </View>
                </SafeAreaView>
            </GestureHandlerRootView>

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
        margin: 15,
        borderWidth: 2,
        borderRadius: 20,
        borderColor: COLORS.third,
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
    button: {
        margin: 15,
        justifyContent: 'center',
        justifySelf: 'stretch',
        marginTop: 10,

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
    commentContent: {
        display: "flex",
        flexDirection: "column",
    },
    row2: {
        margin: 10,
        padding: 20,
        backgroundColor: COLORS.third,
        borderColor: COLORS.third,
        borderRadius: 30,
        justifyContent: 'space-between',
        display: 'flex',
        flexDirection: 'row',

    },
    TextInput: {
        textAlign: "center",
        fontSize: 20,
        color: COLORS.fourth,
        display: 'flex',
        width: '70%',
        marginLeft: 15,
        borderWidth: 2,
        borderRadius: 20,
        borderColor: COLORS.fourth,
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
        margin: 22,
        fontSize: 30,
        color: COLORS.fourth,

    },
    closeIcon: {

        margin: 22,
        fontSize: 30,
        color: COLORS.second,
        justifyContent: "flex-end",
        alignSelf: "flex-end"
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

    buttonCancel: {

        justifyContent: 'center',
        justifySelf: 'center',
        marginTop: 40,
        width: 140,
        height: 40,
        borderRadius: 20,
        backgroundColor: '#646464'
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
