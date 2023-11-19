import React, {FC, useEffect, useRef, useState} from 'react';
import {
    Dimensions, FlatList,
    Image, Modal,
    RefreshControl,
    ScrollView,
    StyleSheet,
    Text,
    TextInput,
    TouchableOpacity,
    View
} from 'react-native';
import {COLORS} from '../Colors';
import store from '../store';
import {API_URL, NOINTERNET, NOTIFICATION, ROUTE, SERVER_ERROR, USER} from "../actions";
import axios from "axios";
import {JwtDecodedResult, RouteInt} from "../Common/Interfaces";
import jwtDecode from "jwt-decode";
import {Swipeable} from "react-native-gesture-handler";

const dimensions = Dimensions.get('window');
const imageHeight = Math.round((dimensions.width * 9) / 16);
const imageWidth = dimensions.width;
interface NotificationInterface{
    Id:number,
    UserId:number,
    Description:string,
    CreatedOn:string,
    Checked:boolean
}
const Notifications: FC<{ navigation: any }> = ({navigation}) => {
    const [notifications, setNotifications] = useState<NotificationInterface[]>([]);

    const [refreshing, setRefreshing] = useState<boolean>(false);
    const [Token, setToken] = useState<JwtDecodedResult>(jwtDecode(store.getState().token));

    const onRefresh = React.useCallback(async () => {
        setRefreshing(true);
        getNotifications()
        setTimeout(() => {
            setRefreshing(false);
        }, 1000);
    }, []);

    async function getNotifications() {
        axios.get(API_URL + USER + "getUserNotifications/" + Token.jti, {headers: {Authorization: store.getState().token}}).then((response) => {

            setNotifications(response.data.Data)
        }).catch((error) => {
        })
    }
    useEffect(() => {
        const unsubscribe = navigation.addListener('focus', () => {
            getNotifications();
        });
        return unsubscribe;

    }, [navigation]);
    const swipeableRef: any = useRef(null);

    const rightSwipeActions = () => {
        return (
            <View
                style={{
                    justifyContent: 'center',

                }}>
                <Text
                    style={{
                        color: 'black',
                        paddingHorizontal: 10,
                        fontWeight: '600',
                        paddingVertical: 20,
                    }}
                >
                    {"Usuń?"}
                </Text>
            </View>
        );
    };
    const closeSwipeable = () => {
        if (swipeableRef != null) {
            if (swipeableRef.current != null) {
                swipeableRef.current.close();
            }
        }
    }
    const swipeFromRightOpen = (item: number) => {
        axios.get(API_URL + USER + "setUserNotification/" + Token.jti+"/"+item, {headers: {Authorization: store.getState().token}}).then((response) => {
            setNotifications(response.data.Data)
        }).catch((error) => {
        })
    };
    function RednderItem(item:NotificationInterface){
    return(
        <Swipeable
            ref={swipeableRef}
            renderLeftActions={rightSwipeActions}
            onSwipeableLeftOpen={() => {
                swipeFromRightOpen(item.Id);
                closeSwipeable()
            }}>
            <View style={styles.card}>
                <Text style={styles.sumText}>{item.Description}</Text>
                <Text style={{justifyContent:"flex-end",alignSelf:"flex-end"}}>{item.CreatedOn}</Text>
            </View>
        </Swipeable>

    );
}
    return (
        <View style={{flex: 1}}>
            <View style={{padding: 25, margin: 10}}>
                <FlatList data={notifications} renderItem={(item)=>RednderItem(item.item)}>
                </FlatList>
            </View>
        </View>
    );
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        justifyContent: 'center',
        display: 'flex',
        flexDirection: 'column',
        backgroundColor: 'white',
    },
    box: {
        justifyContent: 'center',
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        alignContent: 'center',
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
    title: {
        fontSize: 14,
        fontWeight: '500',
        color: COLORS.second,
        marginLeft: 10,
        marginBottom: 10,
        textAlign: 'left',
    },
    sumText: {
        color: COLORS.second,
        fontSize: 18,
        fontWeight: '800',
        marginLeft: 10,
    },
    totalText: {
        color: COLORS.second,
        fontSize: 26,
        fontWeight: '800',
    },
    priceText: {
        color: COLORS.second,
        fontSize: 18,
        fontWeight: '800',
        marginBottom: 15,
    },
    rightBox: {
        alignItems: 'flex-end',
        marginLeft: 100,
    },
    card: {
        backgroundColor: 'white',
        borderRadius: 20,
        padding: 10,
        marginVertical: 20,
    },
    elevation: {
        elevation: 5,
        shadowColor: '#52006A',
    },
    row: {
        justifyContent: 'flex-start',
        display: 'flex',
        flexDirection: 'row',
        alignItems: 'center',
        alignContent: 'center'
        // marginTop: 20,
        // margin: 10,
    },
    border: {
        margin: 10,
    },
    column: {
        justifyContent: 'center',
        display: 'flex',
        flexDirection: 'column',
        flexWrap: 'wrap',
        alignItems: 'center',
        alignContent: 'center',
        marginLeft: 10,
        // maxHeight: 60,
    },
    innerColumn: {
        justifyContent: 'flex-start',
        display: 'flex',
        flexDirection: 'column',
        flexWrap: 'wrap',
        alignItems: 'flex-start',
        alignContent: 'flex-start',
    },
    columnWide: {
        justifyContent: 'center',
        display: 'flex',
        alignItems: 'center',
        alignContent: 'center',
        flex: 1,
        // borderWidth: 1,
        height: imageHeight / 2.5,
    },
    counter: {
        margin: 0,
    },
    counterText: {
        textAlign: 'center',
        fontSize: 20,
        color: 'white',
        fontWeight: '800',
        marginBottom: 10,
        marginTop: 5,
    },
    counterContainer: {
        borderRadius: 20,
        paddingTop: 10,
        paddingBottom: 10,
        backgroundColor: COLORS.second,
        elevation: 10,
    },
    icon: {
        margin: 0,
        fontSize: 24,
        color: 'white',
    },
    imageContainer: {
        justifyContent: 'center',
        display: 'flex',
        flexDirection: 'column',
        flexWrap: 'wrap',
        alignItems: 'center',
        alignContent: 'center',
        width: imageWidth / 4,
        height: imageHeight / 2.25,
        borderRadius: 50,
        elevation: 20,
    },
    image: {
        width: imageWidth / 4,
        height: imageHeight / 2.25,
        margin: 0,
        padding: 0,
        borderRadius: 50,
    },
    button: {
        justifyContent: 'center',
        marginTop: 20,
        width: 250,
        height: 40,
        borderRadius: 20,
        backgroundColor: COLORS.second,
    },
    buttonCode: {
        borderRadius: 20,
        backgroundColor: COLORS.second,
    },
    buttonText: {
        textAlign: 'center',
        color: 'white',
        fontSize: 18,
        fontWeight: '800',
    },
    marginBox: {
        marginTop: 20,
        marginBottom: 20,
    },
    input: {
        color: 'black',
        marginLeft: 10,
    },
    inputMargin: {
        marginBottom: 20,
    },
    infoText: {
        textAlign: 'center',
        color: COLORS.second,
        fontSize: 18,
    },
    emptyText: {
        textAlign: 'center',
        color: "gray",
        fontSize: 20,
    },
});

export default Notifications;
