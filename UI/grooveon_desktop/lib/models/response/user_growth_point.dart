import 'package:json_annotation/json_annotation.dart';

part 'user_growth_point.g.dart';

@JsonSerializable()
class UserGrowthPoint {
  final String label;
  final int count;

  UserGrowthPoint({
    required this.label,
    required this.count,
  });

  factory UserGrowthPoint.fromJson(Map<String, dynamic> json) =>
      _$UserGrowthPointFromJson(json);

  Map<String, dynamic> toJson() => _$UserGrowthPointToJson(this);
}