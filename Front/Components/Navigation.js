import {
    NavigationContainer,
    useNavigation,
    useNavigationContainerRef,
} from '@react-navigation/native';
import {
    createDrawerNavigator,
    DrawerContentScrollView,
    DrawerItem,
    DrawerItemList,
} from '@react-navigation/drawer';
import React, {useEffect, useMemo, useState} from 'react';
import Registration from './Screens/Registration'
import Citys from './Screens/Citys';
import TouristSpotsList from './Screens/TouristSpotsList';
import TouristSpot from './Screens/TouristSpot';
import Login from './Screens/Login';
import Settings from './Screens/Settings';
import {createNativeStackNavigator} from '@react-navigation/native-stack';
import Checkout from './Screens/Route';
import {Image, Touchable, TouchableOpacity, View} from 'react-native';
import {JWT, LOGIN, LOGOUT} from './actions';
import {useDispatch} from 'react-redux';
import store from './store';
import {getData} from './StorageHelper';
import {isTokenExp, LogOut} from './Utilities';
import {COLORS} from './Colors';
import Icon from 'react-native-vector-icons/FontAwesome';
import Ionicons from 'react-native-vector-icons/Ionicons';
import NetInfo from '@react-native-community/netinfo';
import EditProfile from './Screens/EditProfile';
import Map from "./Screens/Map";
import Visited from "./Screens/Visited";
import Notifications from "./Screens/Notifications";
import openMap from 'react-native-open-maps';

function Navigation() {
    const Drawer = createDrawerNavigator();
    const dispatch = useDispatch();
    const Stack = createNativeStackNavigator();
    const [checkOldToken, checked] = useState(false);
    const [Avatar, setAvatar] = useState('');

    useEffect(() => {
        if (!checkOldToken) {
            getData(JWT).then(token => {
                if (token !== null) {
                    if (!isTokenExp(token)) {
                        dispatch({type: LOGIN, payload: token});
                    }
                }
            });
            checked(true);
        }

        async function setAv() {
            if (store.getState().Avatar) {
                setAvatar(await store.getState().Avatar);
            }
        }

        setAv()


    }, [checkOldToken, dispatch]);

    async function SetNewAvatar() {

        if (store.getState().Avatar) {
            setAvatar(await store.getState().Avatar);
        }


    }

    function StackPart() {
        return (
            <Stack.Navigator
                initialRouteName={'Drawer'}
                screenOptions={{
                    tabBarLabelPosition: 'beside-icon',
                    tabBarLabelStyle: {
                        fontSize: 12,
                        fontFamily: 'Ubuntu-Medium',
                        color: COLORS.lightOrangeButton,
                    },
                    tabBarIconStyle: {display: 'none'},
                    tabBarStyle: {
                        backgroundColor: COLORS.mainOrange,
                    },
                    contentStyle: {backgroundColor: 'white'},
                    headerShown: false,
                    backgroundColor: 'white',
                    headerTintColor: COLORS.second,
                }}>
                <Stack.Screen name="EditScreen" component={EditProfile}/>
                <Stack.Screen name="Drawer" component={DrawerPart}/>
                <Stack.Screen name="TouristSpot" component={TouristSpot}/>
                <Stack.Screen name="TouristSpotsList" component={TouristSpotsList}/>
            </Stack.Navigator>
        );
    }

    function CustomDrawerContent(props) {
        return (
            <DrawerContentScrollView {...props}>

                <View
                    style={{
                        justifyContent: 'center',
                        display: 'flex',
                        flexDirection: 'column',
                        flexWrap: 'wrap',
                        alignItems: 'center',
                        alignContent: 'center',
                        margin: 10,
                        borderBottomWidth: 1,
                        borderColor: COLORS.second,
                    }}>
                    {store.getState().isLoggedIn && Avatar != '' ?
                        <TouchableOpacity onPress={() => SetNewAvatar()}>
                            <Image
                                style={{
                                    height: 100,
                                    width: 100,
                                    marginBottom: 10,
                                }}
                                source={{uri: Avatar}}
                            />
                        </TouchableOpacity>
                        :
                        <Image
                            style={{
                                height: 100,
                                width: 100,
                                marginBottom: 10,
                            }}
                            source={require('./Screens/logo.png')}
                        />
                    }
                </View>
                <DrawerItemList {...props} />
                {/*//po liscie z Drawer Part*/}
                {store.getState().isLoggedIn ? (
                    <View>
                        <DrawerItem
                            onPress={() => LogOut(props.navigation, dispatch)}
                            label="Wyloguj się"
                            labelStyle={{fontSize: 15, color: 'black', fontWeight: 'normal'}}
                            icon={() => <Ionicons name="exit" size={22} color="#ccc"/>}
                        />
                    </View>
                ) : null}
            </DrawerContentScrollView>
        );
    }

    function DrawerPart() {
        return (
            <Drawer.Navigator
                screenOptions={{
                    headerPressColor: COLORS.lightOrangeButton,
                    headerShadowVisible: true,
                    headerTintColor: COLORS.second,
                    drawerActiveBackgroundColor: COLORS.second,
                    drawerActiveTintColor: COLORS.main,
                    drawerInactiveTintColor: '#333',
                    drawerLabelStyle: {
                        fontFamily: 'Poppins-Regular',
                        fontSize: 15,
                    },
                    itemStyle: {flex: 1, marginVertical: 5},
                }}
                initialRouteName="Citys"
                drawerContent={props => <CustomDrawerContent {...props} />}>
                <Drawer.Screen
                    options={{
                        title: 'Miasta',
                        drawerIcon: ({focused, size}) => (
                            <Icon
                                name="home"
                                size={size - 2}
                                color={focused ? COLORS.main : '#ccc'}
                            />
                        ),
                    }}
                    name="Citys"
                    component={Citys}
                />
                <Drawer.Screen
                    options={{
                        title: 'Mapa',
                        drawerIcon: ({focused, size}) => (
                            <Icon
                                name="map"
                                size={size - 2}
                                color={focused ? COLORS.main : '#ccc'}
                            />
                        ),
                    }}
                    name="Map"
                    component={Map}
                />
                {store.getState().isLoggedIn ? (
                    <Drawer.Screen
                        options={{
                            title: 'Ustawienia',
                            drawerIcon: ({focused, size}) => (
                                <Icon
                                    name="user-circle-o"
                                    size={size - 3}
                                    color={focused ? COLORS.main : '#ccc'}
                                />
                            ),
                        }}
                        name="Settings"
                        component={Settings}
                    />
                ) : (
                    <Drawer.Screen
                        options={{
                            title: 'Zaloguj się',
                            drawerIcon: ({focused, size}) => (
                                <Icon
                                    name="user-circle-o"
                                    size={size - 3}
                                    color={focused ? COLORS.main : '#ccc'}
                                />
                            ),
                        }}
                        name="Login"
                        component={Login}
                    />
                )}
                {store.getState().isLoggedIn ? (
                        <Drawer.Screen
                            options={{
                                title: 'Odwiedzone',
                                drawerIcon: ({focused, size}) => (
                                    <Icon
                                        name="book"
                                        size={size - 3}
                                        color={focused ? COLORS.main : '#ccc'}
                                    />
                                ),
                            }}
                            name="Visited"
                            component={Visited}
                        />
                    ) :
                    null
                }
                {store.getState().isLoggedIn ? (
                    <Drawer.Screen
                        options={{
                            title: 'Trasy',
                            drawerIcon: ({focused, size}) => (
                                <Icon
                                    name="road"
                                    size={size - 2}
                                    color={focused ? COLORS.main : '#ccc'}
                                />
                            ),
                        }}
                        name="Checkout"
                        component={Checkout}
                    />
                ) : (
                    <Drawer.Screen
                        options={{
                            title: 'Utwórz konto',
                            drawerIcon: ({focused, size}) => (
                                <Icon
                                    name="user-plus"
                                    size={size - 4}
                                    color={focused ? COLORS.main : '#ccc'}
                                />
                            ),
                        }}
                        name="Registration"
                        component={Registration}
                    />
                )}
                {store.getState().isLoggedIn ? (
                    <Drawer.Screen
                        options={{
                            title: 'Powiadomienia',
                            drawerIcon: ({focused, size}) => (
                                <Icon
                                    name="bell"
                                    size={size - 2}
                                    color={focused ? COLORS.main : '#ccc'}
                                />
                            ),
                        }}
                        name="Notification"
                        component={Notifications}
                    />
                ) : null}
            </Drawer.Navigator>
        );
    }

    return (
        <NavigationContainer>
            <StackPart/>
        </NavigationContainer>
    );
}

export default Navigation;
