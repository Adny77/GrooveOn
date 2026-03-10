import 'package:grooveon_desktop/models/user.dart';
import 'package:grooveon_desktop/providers/base_provider.dart';

class UserProvider extends BaseProvider<User> {
  UserProvider() : super("User");

  @override
  User fromJson(dynamic data) => User.fromJson(data);
}
